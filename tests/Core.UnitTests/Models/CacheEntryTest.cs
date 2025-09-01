using Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Text.Json;

namespace Core.UnitTests.Models;

public sealed class CacheEntryTest
{
    private class DummyCacheEntry : ICacheEntry
    {
        public object Key { get; set; } = "testKey";
        public object? Value { get; set; } = "testValue";
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public long? Size { get; set; }
        public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
        public Action<ICacheEntry>? PostEvictionCallbacks { get; set; }
        public IDictionary<string, object?> Properties { get; } = new Dictionary<string, object?>();

        public IList<IChangeToken> ExpirationTokens => throw new NotImplementedException();

        IList<PostEvictionCallbackRegistration> ICacheEntry.PostEvictionCallbacks => throw new NotImplementedException();

        public void Dispose() { }
    }

    [Fact]
    public void DefaultConstructor_InitializesProperties()
    {
        var entry = new CacheEntry();
        Assert.Null(entry.AbsoluteExpiration);
        Assert.Null(entry.AbsoluteExpirationRelativeToNow);
        Assert.Null(entry.Size);
        Assert.Null(entry.SlidingExpiration);
    }

    [Fact]
    public void Constructor_FromICacheEntry_CopiesProperties()
    {
        var dummy = new DummyCacheEntry
        {
            Key = "myKey",
            Value = 123,
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(5),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
            Priority = CacheItemPriority.High,
            Size = 42,
            SlidingExpiration = TimeSpan.FromMinutes(1)
        };

        var entry = new CacheEntry(dummy);

        Assert.Equal(dummy.Key, entry.Key);
        Assert.Equal(JsonSerializer.Serialize(dummy.Value), entry.Value);
        Assert.Equal(dummy.Value.GetType().AssemblyQualifiedName, entry.ValueTypeName);
        Assert.Equal(dummy.AbsoluteExpiration, entry.AbsoluteExpiration);
        Assert.Equal(dummy.AbsoluteExpirationRelativeToNow, entry.AbsoluteExpirationRelativeToNow);
        Assert.Equal(dummy.Priority, entry.Priority);
        Assert.Equal(dummy.Size, entry.Size);
        Assert.Equal(dummy.SlidingExpiration, entry.SlidingExpiration);
    }

    [Fact]
    public void Constructor_FromICacheEntry_ThrowsIfValueIsNull()
    {
        var dummy = new DummyCacheEntry { Value = null };
        Assert.Throws<ArgumentException>(() => new CacheEntry(dummy));
    }

    [Fact]
    public void ValueType_ReturnsCorrectType()
    {
        var entry = new CacheEntry
        {
            ValueTypeName = typeof(int).AssemblyQualifiedName!
        };
        Assert.Equal(typeof(int), entry.ValueType);
    }

    [Fact]
    public void TryGet_ReturnsDefaultIfTypeMismatch()
    {
        var entry = new CacheEntry { Value = "stringValue" };
        var result = entry.TryGet<int>();
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryGet_ReturnsValueIfTypeMatches()
    {
        var entry = new CacheEntry { Value = "stringValue" };
        var result = entry.TryGet<string>();
        Assert.Equal("stringValue", result);
    }
}