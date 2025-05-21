using Core.Configuration;

namespace UnitTests.Configuration;

public class RetrySettingsTest
{
    [Fact]
    public void DefaultValues_ShouldBeSetCorrectly()
    {
        var settings = new RetrySettings();

        Assert.True(settings.Enabled);
        Assert.Equal(3, settings.MaxRetryCount);
        Assert.Equal(1, settings.Delay);
        Assert.Equal(10, settings.MaxDelay);
        Assert.Equal(5, settings.Jitter);
    }

    [Fact]
    public void CanSetProperties()
    {
        var settings = new RetrySettings
        {
            Enabled = false,
            MaxRetryCount = 7,
            Delay = 2,
            MaxDelay = 20,
            Jitter = 8
        };

        Assert.False(settings.Enabled);
        Assert.Equal(7, settings.MaxRetryCount);
        Assert.Equal(2, settings.Delay);
        Assert.Equal(20, settings.MaxDelay);
        Assert.Equal(8, settings.Jitter);
    }
}