using Core.Constants;

namespace Core.Configuration.Tests
{
    public sealed class OpenAiSettingsTest
    {
        [Fact]
        public void DefaultValues_ShouldBeSetCorrectly()
        {
            var settings = new OpenAiSettings();

            Assert.False(settings.ClearCache);
            Assert.Equal(Defaults.OpenAiModel, settings.Model);
            Assert.Null(settings.ApiKey);
            Assert.Equal(Defaults.OpenAiABaseAddress, settings.BaseAddress);
            Assert.Equal(Defaults.OpenAiEndpoint, settings.Endpoint);
            Assert.Equal(Defaults.OpenAiClientName, settings.HttpClientName);
        }

        [Fact]
        public void CanSetProperties()
        {
            var settings = new OpenAiSettings
            {
                ClearCache = true,
                Model = "custom-model",
                ApiKey = "test-key",
                BaseAddress = "https://custom-base/",
                Endpoint = "/custom-endpoint",
                HttpClientName = "CustomClient"
            };

            Assert.True(settings.ClearCache);
            Assert.Equal("custom-model", settings.Model);
            Assert.Equal("test-key", settings.ApiKey);
            Assert.Equal("https://custom-base/", settings.BaseAddress);
            Assert.Equal("/custom-endpoint", settings.Endpoint);
            Assert.Equal("CustomClient", settings.HttpClientName);
        }
    }
}