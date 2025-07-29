using Core.Caching;
using Microsoft.Extensions.Logging;
using Moq;

namespace Core.UnitTests.Caching;

public class FileCacheServiceTest : IDisposable
{
    private readonly string _testCacheDir;
    private readonly Mock<ILogger<FileCacheService>> _loggerMock;
    private readonly FileCacheService _service;

    public FileCacheServiceTest()
    {
        _testCacheDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testCacheDir);
        _loggerMock = new Mock<ILogger<FileCacheService>>();
        _service = new FileCacheService(_loggerMock.Object, _testCacheDir, enabled: true);
    }

    [Fact]
    public async Task CreateEntryAsync_And_TryGetAsync_ShouldStoreAndRetrieveValue()
    {
        var key = "testKey";
        var value = new TestObject { Id = 1, Name = "Test" };

        await _service.CreateEntryAsync(key, value);

        var result = await _service.TryGetAsync<TestObject>(key);

        Assert.NotNull(result);
        Assert.Equal(value.Id, result!.Id);
        Assert.Equal(value.Name, result.Name);
    }

    [Fact]
    public async Task TryGetAsync_ShouldReturnDefault_WhenKeyDoesNotExist()
    {
        var result = await _service.TryGetAsync<TestObject>("nonexistent");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateEntryAsync_ShouldNotStore_WhenDisabled()
    {
        var disabledService = new FileCacheService(_loggerMock.Object, _testCacheDir, enabled: false);
        await disabledService.CreateEntryAsync("disabledKey", new TestObject { Id = 2, Name = "Disabled" });

        var result = await disabledService.TryGetAsync<TestObject>("disabledKey");
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteCacheFile()
    {
        var key = "removeKey";
        var value = new TestObject { Id = 3, Name = "RemoveMe" };
        await _service.CreateEntryAsync(key, value);

        var path = Path.Combine(_testCacheDir, $"{key}.cache");
        Assert.True(File.Exists(path));

        await _service.RemoveAsync(key);

        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Remove_ShouldDeleteFile_WhenEnabled()
    {
        var key = "staticRemove";
        var path = Path.Combine(_testCacheDir, $"{key}.cache");
        File.WriteAllText(path, "dummy");

        FileCacheService.Remove(path);

        Assert.False(File.Exists(path));
    }

    [Fact]
    public void Remove_ShouldNotDeleteFile_WhenDisabled()
    {
        var key = "staticRemoveDisabled";
        var path = Path.Combine(_testCacheDir, $"{key}.cache");
        File.WriteAllText(path, "dummy");

        FileCacheService.Remove(key);

        Assert.True(File.Exists(path));
    }

    [Fact]
    public void SanitizeJsonString_ShouldReturnValidJson()
    {
        var validJson = "{\"Id\":1,\"Name\":\"Test\"}";
        var result = typeof(FileCacheService)
            .GetMethod("SanitizeJsonString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
            .Invoke(null, [validJson]);

        Assert.Equal(validJson, result);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testCacheDir))
        {
            Directory.Delete(_testCacheDir, true);
        }
        GC.SuppressFinalize(this);
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}