using Core.Helpers;

namespace Core.UnitTests.Helpers;

public sealed class CachingHelperTest
{
    [Theory]
    [InlineData("prefix", "key", "hash")] // Example hash, replace with actual
    [InlineData("user", "id123", null)] // Example hash, replace with actual
    public void GenCacheKey_ReturnsExpectedFormat(string prepend, string key, string? hash)
    {
        // Act
        var result = CachingHelper.GenCacheKey(prepend, key, hash);

        // Assert
        Assert.StartsWith(prepend + "_", result);
        Assert.True(result.Length > prepend.Length + 1);
    }

    [Fact]
    public void GenCacheKey_DifferentInputs_ProduceDifferentKeys()
    {
        var key1 = CachingHelper.GenCacheKey("prefix", "key1", "hash1");
        var key2 = CachingHelper.GenCacheKey("prefix", "key2", "hash2");

        Assert.NotEqual(key1, key2);
    }

    [Fact]
    public void GenCacheKey_NullHash_HandledCorrectly()
    {
        var keyWithNullHash = CachingHelper.GenCacheKey("prefix", "key", null);
        var keyWithEmptyHash = CachingHelper.GenCacheKey("prefix", "key", "");

        Assert.NotNull(keyWithNullHash);
        Assert.NotNull(keyWithEmptyHash);
        Assert.Equal(keyWithNullHash, keyWithEmptyHash);
    }
}