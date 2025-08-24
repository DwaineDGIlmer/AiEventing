using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Caching;
using Core.Configuration;
using Core.Constants;
using Core.Contracts;
using Core.Extensions;
using Core.Services;
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
    public void AddCacheService_ShouldAddMemoryCacheService_WhenCachingTypeIsInMemory()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:CachingType", "InMemory" },
                { "ConnectionStrings:AzureWebJobsStorage", "DefaultEndpointsProtocol=https;AccountName=fakestorage;AccountKey=01234567890==;EndpointSuffix=core.windows.net" }
            })
            .Build();

        // Act
        var mockBlobContainerClient = new Mock<BlobContainerClient>();
        mockBlobContainerClient.Setup(m => m.CreateIfNotExistsAsync(
            It.IsAny<PublicAccessType>(),
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContainerInfo>>());
        var mockBlobClient = new Mock<BlobServiceClient>();
        mockBlobClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);
        services.AddCacheLoaderService(config, mockBlobClient.Object);
        services.AddCacheService(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var cacheService = provider.GetService<ICacheService>();
        Assert.NotNull(cacheService);
        Assert.IsType<MemoryCacheService>(cacheService);
        Assert.NotNull(provider.GetService<IMemoryCache>());
    }

    [Fact]
    public void AddCacheService_Should_Load_When_UseCacheLoader_IsNotNull()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:CachingType", "InMemory" },
                { "ConnectionStrings:AzureWebJobsStorage", "1234567890" },
                { "MemoryCacheSettings:UseCacheLoader", "true" }
            })
            .Build();
        var mock = new Mock<BlobContainerClient>();


        // Act
        var mockBlobContainerClient = new Mock<BlobContainerClient>();
        mockBlobContainerClient.Setup(m => m.CreateIfNotExistsAsync(
            It.IsAny<PublicAccessType>(),
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContainerInfo>>());
        var mockBlobClient = new Mock<BlobServiceClient>();
        mockBlobClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);
        services.AddCacheLoaderService(config, mockBlobClient.Object);
        services.AddCacheService(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var cacheService = provider.GetService<ICacheService>();
        Assert.NotNull(cacheService);
        Assert.IsType<MemoryCacheService>(cacheService);
    }

    [Fact]
    public void AddCacheService_ShouldThrow_Exception_When_UseCacheLoader_IsNUll()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AiEventSettings:CachingType", "InMemory" },
                { "ConnectionStrings:AzureWebJobsStorage", "1234567890" },
                { "MemoryCacheSettings:UseCacheLoader", "true" }
            })
            .Build();

        // Act
        services.AddCacheService(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var results = Assert.Throws<FormatException>(() => provider.GetService<ICacheService>());
        Assert.Contains("Settings must be of the form \"name=value\"", results.Message);
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

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => services.AddCacheService(config));
    }

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
                new("AiEventSettings:FaultAnalysisService", "true")
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
                { "AiEventSettings:FaultServiceEnabled", "true" },
                { "AiEventSettings:RcaServiceUrl", "http://rcaservice.com/api" },
                { "AiEventSettings:RcaServiceApiKey", "apikey" }
            })
            .Build();

        // Act
        var settings = ServiceCollectionExtensions.GetAiEventSettings(config);

        // Assert
        Assert.True(settings.WriteIndented);
        Assert.Equal(System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, settings.DefaultIgnoreCondition);
        Assert.True(settings.FaultServiceEnabled);
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
    public void AddResilientHttpClient_ShouldAddHttpClient_WithBasicResilience_WhenNoPolicyName()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ResilientHttpPolicy:HttpTimeout", "15" }
            })
            .Build();

        // Act
        services.AddResilientHttpClient(config, "TestClient");

        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IHttpClientFactory>();

        // Assert
        Assert.NotNull(factory);
        var client = factory.CreateClient("TestClient");
        Assert.NotNull(client);
    }

    [Fact]
    public void AddResilientHttpClient_ShouldAddHttpClient_WithConfigureClientDelegate()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ResilientHttpPolicy:HttpTimeout", "10" }
            })
            .Build();

        // Act
        services.AddResilientHttpClient(config, "ConfiguredClient", configureClient: c => c.Timeout = TimeSpan.FromSeconds(5));
        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IHttpClientFactory>();

        // Assert
        Assert.NotNull(factory);
        var client = factory.CreateClient("ConfiguredClient");
        Assert.Equal(TimeSpan.FromSeconds(5), client.Timeout);
    }

    [Fact]
    public void AddResilientHttpClient_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection? services = null;
        var config = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionExtensions.AddResilientHttpClient(services!, config, "TestClient"));
    }

    [Fact]
    public void AddResilientHttpClient_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? config = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            services.AddResilientHttpClient(config!, "TestClient"));
    }

    [Fact]
    public void AddResilientHttpClient_ShouldThrowArgumentNullException_WhenClientNameIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            services.AddResilientHttpClient(config, null!));
    }

    [Fact]
    public void AddBasicResilienceHandler_ShouldConfigureTimeoutAndCircuitBreaker()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var builder = services.AddHttpClient("TestClient");
        builder.AddBasicResilienceHandler(5);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IHttpClientFactory>();

        // Assert
        Assert.NotNull(factory);
        var client = factory.CreateClient("TestClient");
        Assert.NotNull(client);
        Assert.Equal(TimeSpan.FromSeconds(5), client.Timeout);
    }

    [Fact]
    public void AddBasicResilienceHandler_ShouldUseDefaultTimeout_WhenTimeoutIsZeroOrNegative()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHttpClient("DefaultTimeoutClient");

        // Act
        builder.AddBasicResilienceHandler(0);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IHttpClientFactory>();

        // Assert
        Assert.NotNull(factory);
        var client = factory.CreateClient("DefaultTimeoutClient");
        Assert.NotNull(client);
        Assert.Equal(TimeSpan.FromSeconds(60), client.Timeout);
    }

    [Fact]
    public void AddBasicResilienceHandler_ShouldConfigureTimeoutAndCircuitBreakerOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHttpClient("TestClient");

        // Act
        builder.AddBasicResilienceHandler(10);

        var provider = services.BuildServiceProvider();
        var factory = provider.GetService<IHttpClientFactory>();

        // Assert
        Assert.NotNull(factory);
        var client = factory.CreateClient("TestClient");
        Assert.Equal(TimeSpan.FromSeconds(10), client.Timeout);
    }

    [Fact]
    public void InitializeServices_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "OpenAiSettings:IsEnabled", "true" },
                { "OpenAiSettings:HttpClientName", "OpenAiClient" },
                { "OpenAiSettings:ApiKey", "test-api-key" },
                { "OpenAiSettings:BaseAddress", "https://api.openai.com/" },
                { "OpenAiSettings:Endpoint", "v1/endpoint" },
                { "OpenAiSettings:Model", "gpt-4" },
                { "AiEventSettings:WriteIndented", "true" },
                { "AiEventSettings:DefaultIgnoreCondition", "WhenWritingNull" },
                { "AiEventSettings:FaultServiceEnabled", "true" },
                { "AiEventSettings:CachingType", "InMemory" }
            })
            .Build();

        // Act
        services.InitializeServices(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetService<ICacheService>());
        Assert.NotNull(provider.GetService<IOpenAiChatService>());
        Assert.NotNull(provider.GetService<IEmbeddingService>());
        Assert.NotNull(provider.GetService<IOptions<AiEventSettings>>());
        Assert.NotNull(provider.GetService<IOptions<OpenAiSettings>>());
        Assert.NotNull(provider.GetService<IFaultAnalysisService>());
        Assert.NotNull(provider.GetService<IHttpClientFactory>());
    }

    [Fact]
    public void InitializeServices_ShouldNotRegisterFaultAnalysisService_WhenFaultServiceDisabled()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "OpenAiSettings:IsEnabled", "true" },
                { "OpenAiSettings:ApiKey", "theKey" },
                { "OpenAiSettings:HttpClientName", "OpenAiClient" },
                { "AiEventSettings:FaultServiceEnabled", "false" },
                { "AiEventSettings:CachingType", "InMemory" }
            })
            .Build();

        // Act
        services.InitializeServices(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.Null(provider.GetService<IFaultAnalysisService>());
    }

    [Fact]
    public void InitializeServices_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection? services = null;
        var configuration = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.InitializeServices(services!, configuration));
    }

    [Fact]
    public void InitializeServices_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? configuration = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.InitializeServices(configuration!));
    }

    [Fact]
    public void AddCacheService_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        // Arrange
        IServiceCollection? services = null;
        var config = new ConfigurationBuilder().Build();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddCacheService(services!, config));
    }

    [Fact]
    public void AddCacheService_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var services = new ServiceCollection();
        IConfiguration? config = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => services.AddCacheService(config!));
    }

    [Fact]
    public void AddCacheLoaderService_ShouldRegisterCacheLoaderAndBlobClient_WhenUseCacheLoaderIsTrue_AndServiceClientProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MemoryCacheSettings:UseCacheLoader", "true" },
                { "MemoryCacheSettings:AccountUrl", "https://fakeaccount.blob.core.windows.net/" }
            })
            .Build();
        services.Configure(new Action<MemoryCacheSettings>(options =>
        {
            options.UseCacheLoader = true;
            options.AccountUrl = "https://fakeaccount.blob.core.windows.net/";
            options.Container = "test-container";
            options.BlobName = "test-blob";
            options.Prefix = "test-prefix";
        }));
        var mockBlobServiceClient = new Mock<BlobServiceClient>();

        // Act
        var mockBlobContainerClient = new Mock<BlobContainerClient>();
        mockBlobContainerClient.Setup(m => m.CreateIfNotExistsAsync(
            It.IsAny<PublicAccessType>(),
            It.IsAny<IDictionary<string, string>?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<BlobContainerInfo>>());
        var mockBlobClient = new Mock<BlobServiceClient>();
        mockBlobClient.Setup(m => m.GetBlobContainerClient(It.IsAny<string>())).Returns(mockBlobContainerClient.Object);
        services.AddCacheLoaderService(config, mockBlobClient.Object);
        services.AddLogging();
        var provider = services.BuildServiceProvider();

        // Assert
        var cacheLoader = provider.GetService<ICacheLoader>();
        var cacheBlobClient = provider.GetService<ICacheBlobClient>();
        Assert.NotNull(cacheLoader);
        Assert.NotNull(cacheBlobClient);
        Assert.IsType<CacheLoaderService>(cacheLoader);
        Assert.IsType<BlobCachingService>(cacheBlobClient);
    }

    [Fact]
    public void AddCacheLoaderService_ShouldRegisterCacheLoaderAndBlobClient_WhenUseCacheLoaderIsTrue_AndServiceClientNotProvided()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MemoryCacheSettings:UseCacheLoader", "true" },
                { "MemoryCacheSettings:AccountUrl", "https://fakeaccount.blob.core.windows.net/" }
            })
            .Build();
        services.Configure<MemoryCacheSettings>(new Action<MemoryCacheSettings>(options =>
        {
            options.UseCacheLoader = true;
            options.AccountUrl = "https://fakeaccount.blob.core.windows.net/";
            options.Container = "test-container";
            options.BlobName = "test-blob";
            options.Prefix = "test-prefix";
        }));

        // Act
        services.AddLogging();
        services.AddCacheLoaderService(config, null);
        var provider = services.BuildServiceProvider();

        // Assert
        var cacheLoader = Assert.Throws<FormatException>(() => provider.GetService<ICacheLoader>());
        Assert.NotNull(cacheLoader);
        Assert.Contains("Settings must be of the form \"name=value\"", cacheLoader.Message);
    }

    [Fact]
    public void AddCacheLoaderService_ShouldNotRegisterCacheLoader_WhenUseCacheLoaderIsFalse()
    {
        // Arrange
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MemoryCacheSettings:UseCacheLoader", "false" }
            })
            .Build();

        // Act
        services.AddCacheLoaderService(config, null);
        var provider = services.BuildServiceProvider();

        // Assert
        Assert.Null(provider.GetService<ICacheLoader>());
        Assert.Null(provider.GetService<ICacheBlobClient>());
    }

    [Fact]
    public void AddCacheLoaderService_ShouldNotRegisterCacheLoader_WhenAlreadyRegistered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<ICacheLoader, MockCacheLoader>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "MemoryCacheSettings:UseCacheLoader", "true" },
                { "MemoryCacheSettings:AccountUrl", "https://fakeaccount.blob.core.windows.net/" }
            })
            .Build();

        // Act
        services.AddCacheLoaderService(config, null);
        var provider = services.BuildServiceProvider();

        // Assert
        // Only one registration should exist
        var cacheLoader = provider.GetServices<ICacheLoader>();
        Assert.Single(cacheLoader);
    }

    class MockCacheLoader : ICacheLoader
    {
        public Task<IDictionary<string, object>> LoadCacheAsync()
        {
            return Task.FromResult((IDictionary<string, object>)new Dictionary<string, object>());
        }
        public Task PutAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
        {
            return Task.CompletedTask;
        }
        public Task SaveCacheAsync(IDictionary<string, object>? cache)
        {
            return Task.CompletedTask;
        }
    }
}