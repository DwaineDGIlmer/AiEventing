using Core.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Core.UnitTests.Configuration;

public class AiEventSettingsTest
{
    [Fact]
    public void Default_Constructor_Sets_Default_Values()
    {
        var settings = new AiEventSettings();

        Assert.Equal(JsonIgnoreCondition.WhenWritingNull, settings.DefaultIgnoreCondition);
        Assert.False(settings.WriteIndented);
        Assert.True(settings.LoggingEnabled);
        Assert.True(settings.FaultServiceEnabled);
        Assert.Equal(LogLevel.Information, settings.MinLogLevel);
        Assert.Equal(0, settings.PollingDelay);
        Assert.False(settings.UnsafeRelaxedJsonEscaping);
        Assert.NotNull(settings.ResilientHttpPolicy);
        Assert.NotNull(settings.ResilientHttpPolicy.CircuitBreakerPolicy);
        Assert.NotNull(settings.ResilientHttpPolicy.BulkheadPolicy);
        Assert.NotNull(settings.ResilientHttpPolicy.RetryPolicy);
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
            LoggingEnabled = false,
            FaultServiceEnabled = false,
            MinLogLevel = LogLevel.Warning,
            PollingDelay = 500,
            UnsafeRelaxedJsonEscaping = true,
            ResilientHttpPolicy = new()
            {
                HttpTimeout = 30,
                CircuitBreakerPolicy = cb,
                RetryPolicy = retry,
                BulkheadPolicy = bulkhead,
            },
            AzureTableName = "TestTable",
        };

        Assert.Equal(JsonIgnoreCondition.Never, settings.DefaultIgnoreCondition);
        Assert.False(settings.WriteIndented);
        Assert.False(settings.LoggingEnabled);
        Assert.False(settings.FaultServiceEnabled);
        Assert.Equal(LogLevel.Warning, settings.MinLogLevel);
        Assert.Equal(500, settings.PollingDelay);
        Assert.True(settings.UnsafeRelaxedJsonEscaping);
        Assert.Equal("TestTable", settings.AzureTableName);
        Assert.Equal(30, settings.ResilientHttpPolicy.HttpTimeout);
        Assert.Same(cb, settings.ResilientHttpPolicy.CircuitBreakerPolicy);
        Assert.Same(retry, settings.ResilientHttpPolicy.RetryPolicy);
        Assert.Same(bulkhead, settings.ResilientHttpPolicy.BulkheadPolicy);
    }

    [Fact]
    public void AzureTableName_Defaults_To_Null()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.AzureTableName);
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
        Assert.Equal(60, settings.ResilientHttpPolicy.HttpTimeout);
    }

    [Fact]
    public void Can_Set_HttpTimeout()
    {
        var settings = new AiEventSettings { ResilientHttpPolicy = new() { HttpTimeout = 42 } };
        Assert.Equal(42, settings.ResilientHttpPolicy.HttpTimeout);
    }

    [Fact]
    public void CircuitBreakerSettings_Is_Not_Null_By_Default()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.ResilientHttpPolicy.CircuitBreakerPolicy);
    }

    [Fact]
    public void Can_Set_CircuitBreakerSettings()
    {
        var custom = new CircuitBreakerSettings();
        var settings = new AiEventSettings { ResilientHttpPolicy = new() { CircuitBreakerPolicy = custom } };
        Assert.Same(custom, settings.ResilientHttpPolicy.CircuitBreakerPolicy);
    }

    [Fact]
    public void RetrySettings_Is_Not_Null_By_Default()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.ResilientHttpPolicy.RetryPolicy);
    }

    [Fact]
    public void Can_Set_RetrySettings()
    {
        var custom = new RetrySettings();
        var settings = new AiEventSettings { ResilientHttpPolicy = new() { RetryPolicy = custom } };
        Assert.Same(custom, settings.ResilientHttpPolicy.RetryPolicy);
    }

    [Fact]
    public void BulkheadSettings_Is_Not_Null_By_Default()
    {
        var settings = new AiEventSettings();
        Assert.NotNull(settings.ResilientHttpPolicy.BulkheadPolicy);
    }

    [Fact]
    public void Can_Set_BulkheadSettings()
    {
        var custom = new BulkheadSettings();
        var settings = new AiEventSettings { ResilientHttpPolicy = new() { BulkheadPolicy = custom } };
        Assert.Same(custom, settings.ResilientHttpPolicy.BulkheadPolicy);
    }
}
