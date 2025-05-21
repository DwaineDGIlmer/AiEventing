using Core.Configuration;
using Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;
using System.Net;

namespace UnitTests.Core;

public class ServiceCollectionExtensionsTest
{
    private class TestClass { }

    private class TestClassWithDependency(string value)
    {
        public string Value { get; } = value;
    }

    [Fact]
    public void AddService_ShouldAddSingletonService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddService<TestClass>();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<TestClass>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<TestClass>(service);
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
                new("AiEventSettings:WriteIndented", "true"),
                new("AiEventSettings:DefaultIgnoreCondition", "WhenWritingNull"),
                new("AiEventSettings:ApiKey", "123456789"),
                new("AiEventSettings:ApiUrl", "http://chatgpt.com"),
                new("AiEventSettings:Model", "ChapGpt-Good")
            ])
            .Build();

        // Act
        services.AddSingleton(configuration);
        services.InitializeServices(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var settings = serviceProvider.GetService<AiEventSettings>();

        // Assert
        Assert.NotNull(settings);
        Assert.True(settings.WriteIndented);
        Assert.Equal(System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull, settings.DefaultIgnoreCondition);
    }

    [Fact]
    public void InitializeServices_Bind_Should_Fail_Throw_Exception()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("AiEventSettings:DefaultIgnoreCondition", "NotGood"),
                new KeyValuePair<string, string?>("AiEventSettings:ApiKey", "123456789"),
                new KeyValuePair<string, string?>("AiEventSettings:ApiUrl", "http://chatgpt.com"),
                new KeyValuePair<string, string?>("AiEventSettings:Model", "ChapGpt-Good")
            ])
            .Build();

        // Act
        services.AddSingleton(configuration);
        Assert.Throws<InvalidOperationException>(() => services.InitializeServices(configuration));
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
}
