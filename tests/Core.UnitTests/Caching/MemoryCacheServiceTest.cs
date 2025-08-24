using Core.Caching;
using Core.Configuration;
using Core.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Core.UnitTests.Caching;

public class MemoryCacheServiceTest
{
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly Mock<IOptions<MemoryCacheSettings>> _settingsMock;
    private readonly Mock<ILogger<MemoryCacheService>> _loggerMock;
    private readonly Mock<ICacheLoader> _cacheLoaderMock;

    public MemoryCacheServiceTest()
    {
        _memoryCacheMock = new Mock<IMemoryCache>();
        _settingsMock = new Mock<IOptions<MemoryCacheSettings>>();
        _loggerMock = new Mock<ILogger<MemoryCacheService>>();
        _cacheLoaderMock = new Mock<ICacheLoader>();
    }

    [Fact]
    public async Task TryGetAsync_ReturnsValue_WhenKeyExists()
    {
        var settings = new MemoryCacheSettings { IsEnabled = true };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var key = "testKey";
        var expectedValue = "testValue";
        object boxedValue = expectedValue;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out boxedValue!)).Returns(true);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        var result = await service.TryGetAsync<string>(key);

        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public async Task TryGetAsync_ReturnsNull_WhenKeyDoesNotExist()
    {
        var settings = new MemoryCacheSettings { IsEnabled = true };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var key = "missingKey";
        object boxedValue = null!;

        _memoryCacheMock.Setup(x => x.TryGetValue(key, out boxedValue!)).Returns(false);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        var result = await service.TryGetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEntryAsync_CreatesEntry_WhenEnabled()
    {
        var settings = new MemoryCacheSettings { IsEnabled = true };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var key = "entryKey";
        var value = "entryValue";

        var entryMock = new Mock<ICacheEntry>();
        _memoryCacheMock.Setup(x => x.CreateEntry(key)).Returns(entryMock.Object);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        await service.CreateEntryAsync(key, value);

        entryMock.VerifySet(e => e.Value = value, Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_RemovesEntry_WhenEnabled()
    {
        var settings = new MemoryCacheSettings { IsEnabled = true };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var key = "removeKey";

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        await service.RemoveAsync(key);

        _memoryCacheMock.Verify(x => x.Remove(key), Times.Once);
    }

    [Fact]
    public async Task TryGetAsync_ReturnsDefault_WhenDisabled()
    {
        var settings = new MemoryCacheSettings { IsEnabled = false };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        var result = await service.TryGetAsync<string>("anyKey");

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEntryAsync_DoesNothing_WhenDisabled()
    {
        var settings = new MemoryCacheSettings { IsEnabled = false };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        await service.CreateEntryAsync("key", "value");

        _memoryCacheMock.Verify(x => x.CreateEntry(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RemoveAsync_DoesNothing_WhenDisabled()
    {
        var settings = new MemoryCacheSettings { IsEnabled = false };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object);

        await service.RemoveAsync("key");

        _memoryCacheMock.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Dispose_CallsCacheLoaderSaveCacheAsync()
    {
        var settings = new MemoryCacheSettings { IsEnabled = true };
        _settingsMock.Setup(x => x.Value).Returns(settings);

        var service = new MemoryCacheService(_memoryCacheMock.Object, _settingsMock.Object, _loggerMock.Object, _cacheLoaderMock.Object);

        service.Dispose();

        _cacheLoaderMock.Verify(x => x.SaveCacheAsync(null), Times.AtLeastOnce);
    }
}

