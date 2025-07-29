namespace Core.Contracts;

/// <summary>
/// Defines methods for interacting with a cache to store, retrieve, and remove data.
/// </summary>
/// <remarks>This interface provides asynchronous methods for managing cached data.  Implementations may
/// use various caching mechanisms (e.g., in-memory, distributed)  and should handle serialization and expiration
/// policies as appropriate.</remarks>
public interface ICacheService
{
    /// <summary>
    /// Gets the item associated with this key if present.
    /// </summary>
    /// <param name="key">An object identifying the requested entry.</param>
    /// <returns>True if the key was found.</returns>
    Task<T?> TryGetAsync<T>(string key);

    /// <summary>
    /// Asynchronously sets a value in the cache with the specified key and optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
    /// <param name="key">The unique key used to identify the cached value. Cannot be null or empty.</param>
    /// <param name="value">The value to store in the cache. Cannot be null.</param>
    /// <param name="absoluteExpiration">An optional expiration time for the cached value. If specified, the value will expire after the given
    /// duration. If null, the value will not have an absolute expiration.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task CreateEntryAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);

    /// <summary>
    /// Removes the item associated with the specified key from the underlying storage asynchronously.
    /// </summary>
    /// <param name="key">The key of the item to remove. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task completes when the item is removed or if the key
    /// does not exist.</returns>
    Task RemoveAsync(string key);
}
