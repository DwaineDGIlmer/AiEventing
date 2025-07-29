using Core.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Core.UnitTests.Caching;

public class MemoryCacheServiceTest
{
    [Fact]
    public async Task TryGetAsync_ReturnsValue_WhenKeyExists_AndEnabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, true);
        cache.Set("test-key", 42);

        var result = await service.TryGetAsync<int>("test-key");

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task TryGetAsync_ReturnsNull_WhenKeyDoesNotExist_AndEnabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, true);

        var result = await service.TryGetAsync<string>("missing-key");
        Assert.Null(result);
    }

    [Fact]
    public async Task TryGetAsync_ReturnsDefault_WhenDisabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, false);
        cache.Set("test-key", 42);

        var result = await service.TryGetAsync<int>("test-key");

        Assert.Equal(default, result);
    }

    [Fact]
    public async Task CreateEntryAsync_SetsValue_WhenEnabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, true);

        await service.CreateEntryAsync("new-key", "value");

        Assert.Equal("value", cache.Get("new-key"));
    }

    [Fact]
    public async Task CreateEntryAsync_DoesNothing_WhenDisabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, false);

        await service.CreateEntryAsync("new-key", "value");

        Assert.Null(cache.Get("new-key"));
    }

    [Fact]
    public async Task CreateEntryAsync_SetsValue_WithAbsoluteExpiration()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, true);

        await service.CreateEntryAsync("expiring-key", "expire", TimeSpan.FromSeconds(1));
        Assert.Equal("expire", cache.Get("expiring-key"));

        await Task.Delay(1100);
        Assert.Null(cache.Get("expiring-key"));
    }

    [Fact]
    public async Task RemoveAsync_RemovesEntry_WhenEnabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, true);
        cache.Set("remove-key", 123);

        await service.RemoveAsync("remove-key");

        Assert.Null(cache.Get("remove-key"));
    }

    [Fact]
    public async Task RemoveAsync_DoesNothing_WhenDisabled()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new MemoryCacheService(cache, false);
        cache.Set("remove-key", 123);

        await service.RemoveAsync("remove-key");

        Assert.Equal(123, cache.Get("remove-key"));
    }
}