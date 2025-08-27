using Core.Configuration;
using Core.Contracts;
using Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text;

namespace Core.UnitTests.Services;

public class CacheLoaderServiceTest
{
    private readonly Mock<ICacheBlobClient> _blobClientMock;
    private readonly Mock<ILogger<ICacheLoader>> _loggerMock;
    private readonly IOptions<MemoryCacheSettings> _options;

    public CacheLoaderServiceTest()
    {
        _blobClientMock = new Mock<ICacheBlobClient>();
        _loggerMock = new Mock<ILogger<ICacheLoader>>();
        _options = Options.Create(new MemoryCacheSettings
        {
            CacheKey = null!,
            Prefix = "testprefix",
            BlobName = "testblob"
        });
    }

    [Fact]
    public async Task PutAsync_ShouldAddToInMemoryCache()
    {
        var service = new CacheLoaderService(_blobClientMock.Object, _options, _loggerMock.Object);
        await service.PutAsync("key1", "value1");
        var cache = await service.LoadCacheAsync();
        Assert.True(cache.ContainsKey("key1"));
        Assert.Equal("value1", cache["key1"]);
    }

    [Fact]
    public async Task LoadCacheAsync_ShouldReturnDeserializedCache_WhenBlobExists()
    {
        var dict = new Dictionary<string, object> { { "k", "v" } };
        var json = System.Text.Json.JsonSerializer.Serialize(dict);
        _blobClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        var service = new CacheLoaderService(_blobClientMock.Object, _options, _loggerMock.Object);
        var cache = await service.LoadCacheAsync();

        Assert.NotNull(cache);
        Assert.True(cache.ContainsKey("k"));
        Assert.Equal("v", cache["k"].ToString());
    }

    [Fact]
    public async Task LoadCacheAsync_ShouldReturnInMemoryCache_WhenBlobIsNull()
    {
        _blobClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var service = new CacheLoaderService(_blobClientMock.Object, _options, _loggerMock.Object);
        await service.PutAsync("key2", "value2");
        var cache = await service.LoadCacheAsync();

        Assert.True(cache.ContainsKey("key2"));
        Assert.Equal("value2", cache["key2"]);
    }

    [Fact]
    public async Task LoadCacheAsync_ShouldLogError_OnException()
    {
        _blobClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Blob error"));

        var mockLogger = new MockLogger<CacheLoaderService>(LogLevel.Information);
        var service = new CacheLoaderService(_blobClientMock.Object, _options, mockLogger);
        var cache = await service.LoadCacheAsync();

        Assert.True(mockLogger.Contains("[Error] Failed to load cache from blob storage with key testprefix/testblob. Returning empty cache."));
    }
}