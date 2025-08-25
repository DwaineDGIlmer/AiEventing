using Core.Constants;

namespace Core.Configuration.Tests
{
    public class MemoryCacheSettingsTest
    {
        [Fact]
        public void DefaultValues_AreSetCorrectly()
        {
            var settings = new MemoryCacheSettings();

            Assert.True(settings.IsEnabled);
            Assert.False(settings.UseCacheLoader);
            Assert.Null(settings.AccountUrl);
            Assert.Equal(Defaults.Container, settings.Container);
            Assert.Equal(Defaults.BlobName, settings.BlobName);
            Assert.Equal(string.Empty, settings.CacheKey);
            Assert.Equal(Defaults.Prefix, settings.Prefix);
            Assert.Equal(20, settings.ExpirationInMinutes);
            Assert.Equal(1, settings.DueExpirationInMinutes);
        }

        [Fact]
        public void CanSetProperties()
        {
            var settings = new MemoryCacheSettings
            {
                IsEnabled = false,
                UseCacheLoader = true,
                AccountUrl = "https://example.com",
                Container = "custom-container",
                BlobName = "custom-blob",
                CacheKey = "custom-key",
                Prefix = "custom-prefix",
                ExpirationInMinutes = 30,
                DueExpirationInMinutes = 5
            };

            Assert.False(settings.IsEnabled);
            Assert.True(settings.UseCacheLoader);
            Assert.Equal("https://example.com", settings.AccountUrl);
            Assert.Equal("custom-container", settings.Container);
            Assert.Equal("custom-blob", settings.BlobName);
            Assert.Equal("custom-key", settings.CacheKey);
            Assert.Equal("custom-prefix", settings.Prefix);
            Assert.Equal(30, settings.ExpirationInMinutes);
            Assert.Equal(5, settings.DueExpirationInMinutes);
        }
    }
}