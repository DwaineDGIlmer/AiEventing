using Core.Models;

namespace Core.UnitTests.Models;

// Concrete implementation for testing the abstract RestApiSettings
public class TestRestApiSettings : RestApiSettings
{
    public override string? ApiKey { get; set; }
    public override string BaseAddress { get; set; } = string.Empty;
    public override string Endpoint { get; set; } = string.Empty;
    public override string HttpClientName { get; set; } = string.Empty;
}

public class RestApiSettingsTest
{
    [Fact]
    public void DefaultValues_AreTrue()
    {
        var settings = new TestRestApiSettings();
        Assert.True(settings.IsEnabled);
        Assert.True(settings.IsCachingEnabled);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        var settings = new TestRestApiSettings
        {
            ApiKey = "test-key",
            BaseAddress = "https://api.example.com",
            Endpoint = "/v1/test",
            HttpClientName = "TestClient",
            IsEnabled = false,
            IsCachingEnabled = false
        };

        Assert.Equal("test-key", settings.ApiKey);
        Assert.Equal("https://api.example.com", settings.BaseAddress);
        Assert.Equal("/v1/test", settings.Endpoint);
        Assert.Equal("TestClient", settings.HttpClientName);
        Assert.False(settings.IsEnabled);
        Assert.False(settings.IsCachingEnabled);
    }
}