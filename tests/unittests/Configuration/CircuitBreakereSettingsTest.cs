using Core.Configuration;

namespace UnitTests.Configuration;

public class CircuitBreakerSettingsTest
{
    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        var settings = new CircuitBreakerSettings();

        Assert.True(settings.Enabled);
        Assert.Equal(30, settings.DurationOfBreak);
        Assert.Equal(5, settings.FailureThreshold);
    }

    [Fact]
    public void CanSet_EnabledProperty()
    {
        var settings = new CircuitBreakerSettings { Enabled = false };
        Assert.False(settings.Enabled);

        settings.Enabled = true;
        Assert.True(settings.Enabled);
    }

    [Fact]
    public void CanSet_DurationOfBreakProperty()
    {
        var settings = new CircuitBreakerSettings { DurationOfBreak = 60 };
        Assert.Equal(60, settings.DurationOfBreak);

        settings.DurationOfBreak = 10;
        Assert.Equal(10, settings.DurationOfBreak);
    }

    [Fact]
    public void CanSet_FailureThresholdProperty()
    {
        var settings = new CircuitBreakerSettings { FailureThreshold = 3 };
        Assert.Equal(3, settings.FailureThreshold);

        settings.FailureThreshold = 7;
        Assert.Equal(7, settings.FailureThreshold);
    }
}