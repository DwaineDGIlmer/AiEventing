using Core.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Caching;

/// <summary>
/// Provides an implementation of the <see cref="ICacheService"/> interface using in-memory caching.
/// </summary>
/// <remarks>This service leverages the <see cref="IMemoryCache"/> to store and retrieve data in memory. 
/// It supports asynchronous operations for retrieving, storing, and removing cached items.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="MemoryCacheService"/> class with the specified memory cache.
/// </remarks>
/// <param name="memoryCache">The memory cache instance used to store and retrieve cached data. Cannot be null.</param>
/// <param name="enabled">Indicates whether the caching service is enabled. If false, all cache operations will be no-ops.</param>
public class MemoryCacheService(IMemoryCache memoryCache, bool enabled) : ICacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly bool _enabled = enabled;

    /// <summary>
    /// Asynchronously retrieves a cached value associated with the specified key.
    /// </summary>
    /// <remarks>This method checks the in-memory cache for the specified key and returns the
    /// associated value if found. If the key does not exist in the cache, the result will be <see
    /// langword="null"/>.</remarks>
    /// <typeparam name="T">The type of the value to retrieve from the cache.</typeparam>
    /// <param name="key">The key identifying the cached value. Cannot be null or empty.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the cached value of type
    /// <typeparamref name="T"/> if the key exists in the cache; otherwise, <see langword="null"/>.</returns>
    public Task<T?> TryGetAsync<T>(string key)
    {
        if (_enabled)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }
        return Task.FromResult<T?>(default);
    }

    /// <summary>
    /// Asynchronously sets a value in the memory cache with the specified key and optional expiration time.
    /// </summary>
    /// <remarks>This method stores the specified value in the memory cache using the provided key. If
    /// an absolute expiration is specified, the value will be automatically removed from the cache after the
    /// specified duration. If no expiration is provided, the value will remain in the cache until explicitly
    /// removed or evicted.</remarks>
    /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
    /// <param name="key">The unique key used to identify the cached value. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="value">The value to store in the cache. Cannot be <see langword="null"/>.</param>
    /// <param name="absoluteExpiration">An optional <see cref="TimeSpan"/> specifying the duration after which the cached value will expire. If <see
    /// langword="null"/>, the value will not have an absolute expiration.</param>
    /// <returns>A completed <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task CreateEntryAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        if (_enabled)
        {
            var options = new MemoryCacheEntryOptions() { Size = 2048 };
            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);

            try
            {
                var entry = _memoryCache.CreateEntry(key);
                _memoryCache.Set(key, value, options);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException(nameof(value), "The value cannot be null.");
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes the specified entry from the memory cache.
    /// </summary>
    /// <param name="key">The key of the cache entry to remove. Cannot be <see langword="null"/> or empty.</param>
    /// <returns>A completed <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task RemoveAsync(string key)
    {
        if (_enabled)
        {
            _memoryCache.Remove(key);
        }
        return Task.CompletedTask;
    }
}
