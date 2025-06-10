using Core.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace UnitTests.Configuration;

public class AiEventSettingsTest
{
    [Fact]
    public void Default_Constructor_Sets_Default_Values()
    {
        var settings = new AiEventSettings();

        Assert.Equal(JsonIgnoreCondition.WhenWritingNull, settings.DefaultIgnoreCondition);
        Assert.False(settings.WriteIndented);
        Assert.True(settings.OpenAiEnabled);
        Assert.True(settings.LoggingEnabled);
        Assert.True(settings.RcaServiceEnabled);
        Assert.Equal(LogLevel.Information, settings.MinLogLevel);
        Assert.Equal(0, settings.PollingDelay);
        Assert.False(settings.UnsafeRelaxedJsonEscaping);
        Assert.Equal(30, settings.HttpTimeout);
        Assert.NotNull(settings.CircuitBreakerSettings);
        Assert.NotNull(settings.RetrySettings);
        Assert.NotNull(settings.BulkheadSettings);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        var cb = new CircuitBreakerSettings();
        var retry = new RetrySettings();
        var bulkhead = new BulkheadSettings();

        var settings = new AiEventSettings
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false,
            OpenAiEnabled = false,
            LoggingEnabled = false,
            RcaServiceEnabled = false,
            MinLogLevel = LogLevel.Warning,
            PollingDelay = 500,
            UnsafeRelaxedJsonEscaping = true,
            HttpTimeout = 30,
            CircuitBreakerSettings = cb,
            RetrySettings = retry,
            BulkheadSettings = bulkhead
        };

        Assert.Equal(JsonIgnoreCondition.Never, settings.DefaultIgnoreCondition);
        Assert.False(settings.WriteIndented);
        Assert.False(settings.OpenAiEnabled);
        Assert.False(settings.LoggingEnabled);
        Assert.False(settings.RcaServiceEnabled);
        Assert.Equal(LogLevel.Warning, settings.MinLogLevel);
        Assert.Equal(500, settings.PollingDelay);
        Assert.True(settings.UnsafeRelaxedJsonEscaping);
        Assert.Equal(30, settings.HttpTimeout);
        Assert.Same(cb, settings.CircuitBreakerSettings);
        Assert.Same(retry, settings.RetrySettings);
        Assert.Same(bulkhead, settings.BulkheadSettings);
    }

    [Fact]
    public void UnsafeRelaxedJsonEscaping_Defaults_To_False()
    {
        var settings = new AiEventSettings();
        Assert.False(settings.UnsafeRelaxedJsonEscaping);
    }

    [Fact]
    public void Can_Set_UnsafeRelaxedJsonEscaping()
    {
        var settings = new AiEventSettings { UnsafeRelaxedJsonEscaping = true };
        Assert.True(settings.UnsafeRelaxedJsonEscaping);
    }

    [Fact]
    public void HttpTimeout_Defaults_To_30()
    {
        var settings = new AiEventSettings();
        Assert.Equal(30, settings.HttpTimeout);
    }

    [Fact]
    public void Can_Set_HttpTimeout()
    {
        var settings = new AiEventSettings { HttpTimeout = 42 };
        Assert.Equal(42, settings.HttpTimeout);
    }

    [Fact]
    public void CircuitBreakerSettings_Is_Not_Null_By_Default()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.CircuitBreakerSettings);
    }

    [Fact]
    public void Can_Set_CircuitBreakerSettings()
    {
        var custom = new CircuitBreakerSettings();
        var settings = new AiEventSettings { CircuitBreakerSettings = custom };
        Assert.Same(custom, settings.CircuitBreakerSettings);
    }

    [Fact]
    public void RetrySettings_Is_Not_Null_By_Default()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.RetrySettings);
    }

    [Fact]
    public void Can_Set_RetrySettings()
    {
        var custom = new RetrySettings();
        var settings = new AiEventSettings { RetrySettings = custom };
        Assert.Same(custom, settings.RetrySettings);
    }

    [Fact]
    public void BulkheadSettings_Is_Not_Null_By_Default()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.BulkheadSettings);
    }

    [Fact]
    public void Can_Set_BulkheadSettings()
    {
        var custom = new BulkheadSettings();
        var settings = new AiEventSettings { BulkheadSettings = custom };
        Assert.Same(custom, settings.BulkheadSettings);
    }
}
