using Core.Caching;
using Core.Configuration;
using Core.Constants;
using Core.Contracts;
using Core.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Polly.CircuitBreaker;
using System.Net;

namespace Core.UnitTests.Extensions;

public class ServiceCollectionExtensionsTest
{
    [Fact]
    public void AddService_WithFactory_ShouldAddSingletonService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddService(sp => new TestClassWithDependency("TestValue"));

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<TestClassWithDependency>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<TestClassWithDependency>(service);
        Assert.Equal("TestValue", service.Value);
    }

    [Fact]
    public void InitializeServices_ShouldConfigureServicesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new("OpenAiSettings:Endpoint", Defaults.OpenAiEndpoint),
                new("OpenAiSettings:BaseAddress", Defaults.OpenAiABaseAddress),
                new("OpenAiSettings:ApiKey", "ApiKey"),
                new("AiEventSettings:WriteIndented", "true"),
                new("AiEventSettings:DefaultIgnoreCondition", "WhenWritingNull"),
                new("AiEventSettings:RcaServiceClient", "RcaServiceClient"),
                new("AiEventSettings:RcaServiceUrl", "http://rcaservice.com/api"),
                new("AiEventSettings:RcaServiceApiKey", "987654321"),
                new("AiEventSettings:RcaServiceEnabled", "true")
            ])
            .Build();

        // Act
        services.Configure<AiEventSettings>(configuration);
        services.AddSingleton(sp => new Mock<ICacheService>().Object);
        services.InitializeServices(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiEventSettings>>();
        var httpFactory = serviceProvider.GetService<IHttpClientFactory>();
        var faultAnalysisService = serviceProvider.GetService<IFaultAnalysisService>();

        // Assert
        Assert.NotNull(options);
        Assert.NotNull(options.Value);

        var settings = options.Value;
        Assert.True(settings.WriteIndented);
        Assert.Equal(System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, settings.DefaultIgnoreCondition);

        Assert.NotNull(httpFactory);
        Assert.NotNull(faultAnalysisService);
    }

    [Fact]
    public void GetJsonIgnoreCondition_Should_Revert_To_Default()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new("AiSettings:DefaultIgnoreCondition", "NotGood")
            ])
            .Build();

        // Act       
        var defaultIgnoreCondition = ServiceCollectionExtensions.GetJsonIgnoreCondition(configuration);

        // Assert
        Assert.Equal(System.Text.Json.Serialization.JsonIgnoreCondition.Never, defaultIgnoreCondition);
    }


    [Fact]
    public async Task GetRetryPolicy_ShouldRetryAndEventuallySucceed()
    {
        var settings = new RetrySettings
        {
            Enabled = true,
            MaxRetryCount = 2,
            Delay = 0,
            MaxDelay = 1,
            Jitter = 0
        };

        int callCount = 0;
        var policy = ServiceCollectionExtensions.GetRetryPolicy(settings);

        var result = await policy.ExecuteAsync(() =>
        {
            callCount++;
            if (callCount < 3)
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        Assert.Equal(3, callCount); // 2 retries + 1 initial
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task GetBulkheadPolicy_ShouldReturn503WhenFull()
    {
        var settings = new BulkheadSettings
        {
            Enabled = true,
            MaxParallelization = 1,
            MaxQueuingActions = 0
        };

        var policy = ServiceCollectionExtensions.GetBulkheadPolicy(settings);

        // Start one task to occupy the only slot
        var tcs = new TaskCompletionSource();
        var runningTask = policy.ExecuteAsync(async () =>
        {
            await tcs.Task;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        // Second task should be rejected and fallback to 503
        var rejectedResponse = await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, rejectedResponse.StatusCode);

        tcs.SetResult();
        await runningTask;
    }

    [Fact]
    public async Task GetCircuitBreakerPolicy_ShouldOpenAfterFailures()
    {
        var settings = new CircuitBreakerSettings
        {
            Enabled = true,
            FailureThreshold = 2,
            DurationOfBreak = 1
        };

        var policy = ServiceCollectionExtensions.GetCircuitBreakerPolicy(settings);

        // Cause enough failures to open the circuit
        await Assert.ThrowsAsync<BrokenCircuitException<HttpResponseMessage>>(async () =>
        {
            await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
            await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
            await policy.ExecuteAsync(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
        });
    }

    [Fact]
    public void GetAiEventSettings_ShouldBindConfigurationCorrectly()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:WriteIndented", "true" },
                { "AiEventSettings:DefaultIgnoreCondition", "WhenWritingNull" },
                { "AiEventSettings:RcaServiceEnabled", "true" },
                { "AiEventSettings:RcaServiceUrl", "http://rcaservice.com/api" },
                { "AiEventSettings:RcaServiceApiKey", "apikey" }
            })
            .Build();

        // Act
        var settings = ServiceCollectionExtensions.GetAiEventSettings(config);

        // Assert
        Assert.True(settings.WriteIndented);
        Assert.Equal(System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, settings.DefaultIgnoreCondition);
        Assert.True(settings.RcaServiceEnabled);
        Assert.Equal("http://rcaservice.com/api", settings.RcaServiceUrl);
        Assert.Equal("apikey", settings.RcaServiceApiKey);
    }

    [Fact]
    public void GetOpenAiSettings_ShouldBindConfigurationAndEnvironmentVariables()
    {
        // Arrange
        Environment.SetEnvironmentVariable("OPENAI_API_KEY", "envkey");
        Environment.SetEnvironmentVariable("OPENAI_MODEL", "envmodel");
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "OpenAiSettings:IsEnabled", "true" },
                { "OpenAiSettings:BaseAddress", "" },
                { "OpenAiSettings:Endpoint", "" },
                { "OpenAiSettings:ApiKey", "" },
                { "OpenAiSettings:Model", "" }
            })
            .Build();

        // Act
        var settings = ServiceCollectionExtensions.GetOpenAiSettings(config);

        // Assert
        Assert.True(settings.IsEnabled);
        Assert.Equal(Defaults.OpenAiABaseAddress, settings.BaseAddress);
        Assert.Equal(Defaults.OpenAiEndpoint, settings.Endpoint);
        Assert.Equal("envkey", settings.ApiKey);
        Assert.Equal("envmodel", settings.Model);
    }

    [Fact]
    public void GetSettings_ShouldBindGenericTypeFromConfiguration()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "TestClass:Value", "abc" }
            })
            .Build();

        // Act
        var result = config.GetSettings<TestClass>();

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void GetResilientHttpPolicy_ShouldBindConfiguration()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ResilientHttpPolicy:HttpTimeout", "42" }
            })
            .Build();

        // Act
        var policy = ServiceCollectionExtensions.GetResilientHttpPolicy(config);

        // Assert
        Assert.Equal(42, policy.HttpTimeout);
    }

    [Fact]
    public void GetFileCaching_ShouldReturnFileCacheService()
    {
        // Arrange
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:EnableCaching", "true" }
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        // Act
        var cacheService = ServiceCollectionExtensions.GetFileCaching(config, provider, "TestCache");

        // Assert
        Assert.NotNull(cacheService);
    }

    private class TestClass
    {
        public string Value { get; set; } = string.Empty;
    }

    private class TestClassWithDependency(string value)
    {
        public string Value { get; } = value;
    }

    [Fact]
    public void AddCacheService_ShouldAddMemoryCacheService_WhenCachingTypeIsInMemory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:CachingType", "InMemory" }
            })
            .Build();

        // Act
        services.AddCacheService(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var cacheService = provider.GetService<ICacheService>();
        Assert.NotNull(cacheService);
        Assert.IsType<MemoryCacheService>(cacheService);
        Assert.NotNull(provider.GetService<IMemoryCache>());
    }

    [Fact]
    public void AddCacheService_ShouldAddFileCacheService_WhenCachingTypeIsFileSystem()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
            { "AiEventSettings:CachingType", "FileSystem" }
            })
            .Build();

        // Act
        services.AddCacheService(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var cacheService = provider.GetService<ICacheService>();
        Assert.NotNull(cacheService);
        Assert.IsType<FileCacheService>(cacheService);
    }

    [Fact]
    public void AddCacheService_ShouldNotAddCacheService_WhenCachingTypeIsUnknown()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:CachingType", "UnknownType" }
            })
            .Build();

        // Act
        Assert.Throws<InvalidOperationException>(() => services.AddCacheService(config));
    }
}