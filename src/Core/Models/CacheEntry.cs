using Core.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Models;

/// <summary>
/// Represents a cache entry with associated metadata, such as expiration policies, priority, and size.
/// </summary>
/// <remarks>This class encapsulates the data and metadata for a cache entry, including its value, expiration
/// settings, priority, and size. It provides mechanisms to manage cache entries effectively, such as absolute and
/// sliding expiration policies, and allows retrieval of the value in a strongly-typed manner.</remarks>
public class CacheEntry
{
    /// <summary>
    /// Gets or sets the value of the current instance.
    /// </summary>
    public string Value { get; set; } = default!;

    /// <summary>
    /// Gets the type of the value associated with this Value.
    /// </summary>
    public string ValueTypeName { get; set; } = default!;

    /// <summary>
    /// Gets the runtime type of the value represented by this instance.
    /// </summary>
    /// <remarks>This property retrieves the type using the fully qualified type name stored in
    /// <c>ValueTypeName</c>.  It assumes that the type name is valid and resolvable at runtime.</remarks>
    [JsonIgnore]
    public Type ValueType => Type.GetType(ValueTypeName)!;

    /// <summary>
    /// Gets or sets the absolute expiration date and time for the cache entry.
    /// </summary>
    /// <remarks>Use this property to specify a fixed expiration time for the cache entry.  If both absolute
    /// and sliding expiration are set, the entry will expire based on whichever occurs first.</remarks>
    public DateTimeOffset? AbsoluteExpiration { get; set; }

    /// <summary>
    /// Gets or sets the relative time from now after which the cache entry will expire.
    /// </summary>
    /// <remarks>This property is typically used to specify a sliding expiration policy for cache entries. 
    /// Setting this property to a non-<see langword="null"/> value ensures that the cache entry will  expire after the
    /// specified duration, regardless of activity.</remarks>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    /// <summary>
    /// Gets the key associated with the current object.
    /// </summary>
    public object Key { get; set; } = default!;

    /// <summary>
    /// Gets or sets the priority level of the cache item, which determines how the item is treated during cache
    /// eviction.
    /// </summary>
    /// <remarks>Use this property to specify the importance of the cache item relative to others.  Items with
    /// a higher priority are retained longer during memory pressure scenarios.</remarks>
    public CacheItemPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets the size of the item, in bytes.
    /// </summary>
    public long? Size { get; set; }

    /// <summary>
    /// Gets or sets the sliding expiration time for the cache entry.
    /// </summary>
    /// <remarks>Sliding expiration resets the expiration timer each time the cache entry is accessed. Use
    /// this property to specify how long the cache entry should remain active since its last access.</remarks>
    public TimeSpan? SlidingExpiration { get; set; } = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheEntry"/> class.
    /// </summary>
    /// <remarks>This constructor creates a default instance of the <see cref="CacheEntry"/> class. Use this
    /// constructor to create a new cache entry with default settings.</remarks>
    public CacheEntry() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheEntry"/> class by copying the properties of the specified
    /// cache entry.
    /// </summary>
    /// <remarks>This constructor initializes the <see cref="CacheEntry"/> instance by copying the key, value,
    /// and other metadata from the provided <see cref="ICacheEntry"/>.  It ensures that the source cache entry is valid
    /// and contains a non-null value.</remarks>
    /// <param name="cacheEntry">The source <see cref="ICacheEntry"/> to copy properties from. Cannot be <see langword="null"/> and must have a
    /// non-null <see cref="ICacheEntry.Value"/>.</param>
    /// <exception cref="ArgumentException">Thrown if the <see cref="ICacheEntry.Value"/> of <paramref name="cacheEntry"/> is <see langword="null"/>.</exception>
    public CacheEntry(ICacheEntry cacheEntry)
    {
        cacheEntry.IsNullThrow(nameof(cacheEntry));
        if (cacheEntry.Value is null)
            throw new ArgumentException($"Cache entry value is null.");

        Key = cacheEntry.Key;
        Value = JsonSerializer.Serialize(cacheEntry.Value);
        ValueTypeName = cacheEntry.Value.GetType().AssemblyQualifiedName!;
        AbsoluteExpiration = cacheEntry.AbsoluteExpiration;
        AbsoluteExpirationRelativeToNow = cacheEntry.AbsoluteExpirationRelativeToNow;
        Priority = cacheEntry.Priority;
        Size = cacheEntry.Size;
        SlidingExpiration = cacheEntry.SlidingExpiration;
    }

    /// <summary>
    /// Attempts to retrieve the current value as the specified type.
    /// </summary>
    /// <typeparam name="TOut">The type to which the value should be cast.</typeparam>
    /// <returns>The value cast to <typeparamref name="TOut"/> if the cast is successful; otherwise, <see langword="null"/> or
    /// the default value of <typeparamref name="TOut"/>.</returns>
    public TOut? TryGet<TOut>()
    {
        if (Value is TOut tOutValue)
            return tOutValue;
        return default;
    }
}
