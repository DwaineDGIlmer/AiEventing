using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Caching;

/// <summary>
/// Provides an implementation of <see cref="ICacheService"/> using in-memory caching.
/// </summary>
/// <remarks>This service leverages <see cref="IMemoryCache"/> to store and retrieve cached data. It supports
/// asynchronous operations for retrieving, adding, and removing cache entries. The service can be configured using <see
/// cref="MemoryCacheSettings"/> to enable or disable caching globally. An optional <see cref="ICacheLoader"/> can be
/// provided to handle additional cache loading logic.</remarks>
public sealed class MemoryCacheService : ICacheService, IDisposable
{
    private readonly bool _enabled;
    private readonly ICacheLoader? _cacheLoader;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;
    private readonly CacheHandler? _cacheHandler;
    private readonly Timer? _saveCachedItems;
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheService"/> class, configuring the memory cache and
    /// optional cache loader based on the provided settings.
    /// </summary>
    /// <remarks>If caching is disabled in the provided settings, the cache loader will not be used, even if
    /// it is provided. Ensure that the <paramref name="memoryCache"/> and <paramref name="settings"/> parameters are
    /// properly initialized before calling this constructor.</remarks>
    /// <param name="memoryCache">The memory cache instance used to store cached items. Cannot be <see langword="null"/>.</param>
    /// <param name="settings">The configuration settings for the memory cache. Cannot be <see langword="null"/> and must specify whether
    /// caching is enabled.</param>
    /// <param name="logger">The logger instance used for logging cache operations. Cannot be <see langword="null"/>.</param>
    /// <param name="cacheLoader">An optional cache loader responsible for saving cached items periodically. If provided and caching is enabled,
    /// cached items will be saved every 20 minutes.</param>
    public MemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<MemoryCacheSettings> settings,
        ILogger<MemoryCacheService> logger,
        ICacheLoader? cacheLoader = null)
    {
        _cacheLoader = cacheLoader;
        _logger = logger.IsNullThrow(nameof(logger));
        _memoryCache = memoryCache.IsNullThrow(nameof(memoryCache));
        _enabled = settings.Value.IsNullThrow(nameof(settings)).IsEnabled;
        if (_enabled && settings.Value.UseCacheLoader && _cacheLoader is not null)
        {
            // Load cached items from warm storage on startup
            InitializeCacheAsync();

            // Set up periodic saving of cached items every 20 minutes
            _cacheHandler = new CacheHandler(_cacheLoader);
            _saveCachedItems = new Timer(async _ => await _cacheLoader.SaveCacheAsync(null), null, TimeSpan.FromMinutes(settings.Value.DueExpirationInMinutes), TimeSpan.FromMinutes(settings.Value.ExpirationInMinutes));
        }
    }

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
        if (_enabled && !string.IsNullOrEmpty(key) && value is not null)
        {
            var options = new MemoryCacheEntryOptions() { Size = 2048 };
            if (absoluteExpiration.HasValue)
                options.SetAbsoluteExpiration(absoluteExpiration.Value);

            try
            {
                using var entry = _memoryCache.CreateEntry(key);
                entry.SetOptions(options);
                entry.Value = value;
                _cacheLoader?.PutAsync(key, new CacheEntry(entry), absoluteExpiration);
            }
            catch (ArgumentNullException)
            {
                _logger.LogError("Failed to create cache entry. Key or value is null.");
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

    /// <summary>
    /// Releases resources used by the current instance of the class.
    /// </summary>
    /// <remarks>This method should be called when the instance is no longer needed to ensure proper cleanup
    /// of resources.</remarks>
    public void Dispose()
    {
        _cacheLoader?.SaveCacheAsync(null);
        GC.SuppressFinalize(this);
    }

    private void InitializeCacheAsync()
    {
        if (_enabled && _cacheLoader is not null)
        {
            _logger.LogInformation("Loading cached items from warm storage.");

            var cachedItems = _cacheLoader.LoadCacheAsync().GetAwaiter().GetResult();
            if (cachedItems is null)
            {
                return;
            }

            foreach (var item in cachedItems)
            {
                var objString = item.Value?.ToString();
                if (string.IsNullOrEmpty(objString))
                    continue;

                try
                {
                    var cacheItem = JsonSerializer.Deserialize<CacheEntry>(objString, _options);
                    if (cacheItem is not null)
                    {
                        var options = new MemoryCacheEntryOptions()
                        {
                            Size = cacheItem.Size,
                            SlidingExpiration = cacheItem.SlidingExpiration,
                            AbsoluteExpiration = cacheItem.AbsoluteExpiration,
                            AbsoluteExpirationRelativeToNow = cacheItem.AbsoluteExpirationRelativeToNow,
                            Priority = cacheItem.Priority
                        };

                        using var entry = _memoryCache.CreateEntry(item.Key);
                        entry.SetOptions(options);
                        entry.Value = JsonSerializer.Deserialize(cacheItem.Value, cacheItem.ValueType, _options);
                    }
                }
                catch (JsonException)
                {
                    _logger.LogError("Failed to deserialize cache entry for key {CacheKey}.", item.Key);
                }
            }
        }
    }
}

/// <summary>
/// Provides functionality to manage and persist cache data using a specified cache loader.
/// </summary>
/// <remarks>This class acts as a handler for cache operations, delegating the actual persistence logic to an
/// implementation of <see cref="ICacheLoader"/>. It is designed to simplify cache management by abstracting the
/// underlying storage mechanism.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="CacheHandler"/> class with the specified cache loader.
/// </remarks>
/// <param name="cacheLoader">The cache loader responsible for loading and managing cache data.  This parameter cannot be <see
/// langword="null"/>.</param>
public sealed class CacheHandler(ICacheLoader cacheLoader)
{
    private readonly ICacheLoader _cacheLoader = cacheLoader;

    /// <summary>
    /// Saves the current cache state asynchronously.
    /// </summary>
    /// <remarks>This method triggers the cache-saving process using the underlying cache loader.  It performs
    /// the operation asynchronously and does not take any parameters.</remarks>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task SaveCache()
    {
        await _cacheLoader.SaveCacheAsync(null);
    }
}