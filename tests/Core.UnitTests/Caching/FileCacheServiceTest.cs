using Core.Caching;
using Core.Configuration;
using Core.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Core.UnitTests.Caching;

public sealed class FileCacheServiceTest : IDisposable
{
    private static readonly string _testCacheDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly Mock<ILogger<FileCacheService>> _loggerMock;
    private readonly FileCacheService _service;
    private readonly IOptions<AiEventSettings> _options = Options.Create(new AiEventSettings()
    { 
        CacheLocation = _testCacheDir,
        EnableCaching = true,
    });

    public FileCacheServiceTest()
    {
        Directory.CreateDirectory(_testCacheDir);
        _loggerMock = new Mock<ILogger<FileCacheService>>();
        _service = new FileCacheService(_loggerMock.Object, _options);
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
        var options = Options.Create(new AiEventSettings()
        {
            CacheLocation = _testCacheDir,
            EnableCaching = false
        });
        var disabledService = new FileCacheService(_loggerMock.Object, options);
        await disabledService.CreateEntryAsync("disabledKey", new TestObject { Id = 2, Name = "Disabled" });

        var result = await disabledService.TryGetAsync<TestObject>("disabledKey");
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteCacheFile()
    {
        var key = "removeKey";
        var value = new TestObject { Id = 3, Name = "RemoveMe" };
        var path = FileCacheService.GetFilePath(key, _testCacheDir);

        await _service.CreateEntryAsync(key, value);
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

    [Fact]
    public void GetFilePath_Should_Throw_Exception()
    {
        Assert.Throws<ArgumentNullException>(() => FileCacheService.GetFilePath(null!, _testCacheDir));
        Assert.Throws<ArgumentNullException>(() => FileCacheService.GetFilePath("Tests", null!));
        Assert.Throws<ArgumentException>(() => FileCacheService.GetFilePath("Tests", Path.GetInvalidPathChars().FirstOrDefault().ToString()));
    }

    [Theory]
    [InlineData("simpleKey")]
    [InlineData("key/with:invalid*chars?")]
    [InlineData("anotherKey")]
    [InlineData("inv|alid")]
    public void GetFilePath_ShouldReturnExpectedPath(string key)
    {
        // Act
        var result = FileCacheService.GetFilePath(key, _testCacheDir);

        // Assert        
        Assert.False(ContainAny(result, Path.GetInvalidPathChars()));

        var fileName = Path.GetFileName(result);
        if (key.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            // Should use sanitized name
            var expectedFileName = $"{key.FileSystemName()}.cache";
            Assert.Equal(expectedFileName, fileName);
            Assert.DoesNotContain(key, fileName);
        }
        else
        {
            var expectedFileName = $"{key}.cache";
            Assert.Equal(expectedFileName, fileName);
            Assert.Contains(key, fileName);
        }
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Extension for test assertions
    static bool ContainAny(string input, char[] invalidChars)
    {
        foreach (var c in invalidChars)
        {
            if (input.Contains(c))
                return true;
        }
        return false;
    }
    public void Dispose()
    {
        if (Directory.Exists(_testCacheDir))
        {
            Directory.Delete(_testCacheDir, true);
        }
        GC.SuppressFinalize(this);
    }
}