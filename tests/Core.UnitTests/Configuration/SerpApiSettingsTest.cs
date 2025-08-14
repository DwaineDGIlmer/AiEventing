using Core.Configuration;
using Core.Constants;

namespace Core.UnitTests.Configuration;

public class SerpApiSettingsTest
{
    [Fact]
    public void Default_Values_Are_Correct()
    {
        var settings = new SerpApiSettings();

        Assert.Equal(1440, settings.CacheExpirationInMinutes);
        Assert.Equal(Defaults.SerpApiQuery, settings.Query);
        Assert.Equal(Defaults.SerpApiLocation, settings.Location);
        Assert.Null(settings.ApiKey);
        Assert.Equal(Defaults.SerpApiBaseAddress, settings.BaseAddress);
        Assert.Equal(Defaults.SearchEndpoint, settings.Endpoint);
        Assert.Equal(Defaults.SerpApiClientName, settings.HttpClientName);
    }

    [Fact]
    public void Can_Set_And_Get_Properties()
    {
        var settings = new SerpApiSettings
        {
            CacheExpirationInMinutes = 60,
            Query = "test query",
            Location = "test location",
            ApiKey = "test-api-key",
            BaseAddress = "https://test.com",
            Endpoint = "/test-endpoint",
            HttpClientName = "TestClient"
        };

        Assert.Equal(60, settings.CacheExpirationInMinutes);
        Assert.Equal("test query", settings.Query);
        Assert.Equal("test location", settings.Location);
        Assert.Equal("test-api-key", settings.ApiKey);
        Assert.Equal("https://test.com", settings.BaseAddress);
        Assert.Equal("/test-endpoint", settings.Endpoint);
        Assert.Equal("TestClient", settings.HttpClientName);
    }
}