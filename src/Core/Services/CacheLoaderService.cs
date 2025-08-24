using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using System.Collections.Concurrent;

namespace Core.Services;

/// <summary>
/// Provides functionality to load and save cache data to and from blob storage.
/// </summary>
/// <remarks>This service is designed to handle the serialization and deserialization of cache data as
/// JSON objects, enabling storage and retrieval from a blob storage backend. It supports uploading a dictionary of
/// key-value pairs to blob storage and retrieving them as a dictionary. The service ensures that existing blobs are
/// overwritten during uploads and returns an empty dictionary if no data is available during retrieval.</remarks>
public class CacheLoaderService : ICacheLoader
{
    private readonly ConcurrentDictionary<string, object> _inMemoryCache = [];
    private readonly ICacheBlobClient _cacheBlobClient;
    private readonly ILogger _logger;
    private readonly string _bloblCacheKey;


    /// <summary>
    /// Initializes a new instance of the <see cref="CacheLoaderService"/> class,  which is responsible for managing
    /// cache loading operations using a blob storage client.
    /// </summary>
    /// <remarks>The cache key is determined based on the provided <paramref name="options"/>.  If the
    /// <c>CacheKey</c> property in <paramref name="options"/> is not set, a default key is generated  using the
    /// <c>Prefix</c> and <c>BlobName</c> properties.</remarks>
    /// <param name="cacheBlobClient">The client used to interact with the blob storage for cache operations.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <param name="options">The configuration settings for the memory cache, including cache key, prefix, and blob name.  This parameter
    /// cannot be <see langword="null"/> and must contain a valid <see cref="MemoryCacheSettings"/> value.</param>
    /// <param name="logger">The logger instance used for logging cache loading operations.  This parameter cannot be <see langword="null"/>.</param>
    public CacheLoaderService(
        ICacheBlobClient cacheBlobClient,
        IOptions<MemoryCacheSettings> options,
        ILogger<ICacheLoader> logger)
    {
        options.IsNullThrow(nameof(options));
        options.Value.IsNullThrow(nameof(options.Value));

        _logger = logger.IsNullThrow(nameof(logger));
        _cacheBlobClient = cacheBlobClient.IsNullThrow(nameof(cacheBlobClient));
        _bloblCacheKey = string.IsNullOrEmpty(options.Value.CacheKey) ? $"{options.Value.Prefix.TrimEnd('/')}/cache/{options.Value.BlobName}".TrimStart('/') : options.Value.CacheKey;
    }

    /// <summary>
    /// Asynchronously stores a value in the cache with the specified key and optional expiration time.
    /// </summary>
    /// <param name="key">The unique key used to identify the cached value. Cannot be null or empty.</param>
    /// <param name="value">The value to store in the cache. Cannot be null.</param>
    /// <param name="absoluteExpiration">An optional expiration time for the cached value. If specified, the value will expire after the given
    /// duration. If null, the value will not have an expiration time.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task PutAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        if (!string.IsNullOrEmpty(key) && value is not null)
        {
            _inMemoryCache[key] = value;
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously loads the cache data from a remote source never storage waiting to be saved.
    /// </summary>
    /// <remarks>This method retrieves the cache data as a dictionary of key-value pairs from a remote
    /// blob storage. If the cache is empty or unavailable, an empty dictionary is returned.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary of cache data,
    /// where the keys are strings and the values are objects. If no data is available, an empty dictionary is
    /// returned.</returns>
    public async Task<IDictionary<string, object>> LoadCacheAsync()
    {
        try
        {
            // Attempt to download the cache data from blob storage
            var download = await _cacheBlobClient.GetAsync(_bloblCacheKey);
            if (download is not null)
            {
                // Deserialize the JSON data into a dictionary
                var cacheData = JsonSerializer.Deserialize<Dictionary<string, object>>(download);

                //Delete the blob after loading the cache to prevent reloading stale data   
                if (cacheData is not null)
                {
                    _ = Task.Run(async () => await _cacheBlobClient.DeleteAsync(_bloblCacheKey));
                }

                // Return the loaded cache data or the in-memory cache if deserialization fails
                return cacheData ?? _inMemoryCache.ToDictionary();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load cache from blob storage with key {CacheKey}. Returning empty cache.", _bloblCacheKey);
        }
        return _inMemoryCache;
    }

    /// <summary>
    /// Saves the current cache data to a remote storage location.
    /// </summary>
    /// <remarks>The cache data is serialized to JSON format before being saved. Ensure that the
    /// provided dictionary contains  serializable objects to avoid serialization errors.</remarks>
    /// <param name="cache">An optional dictionary containing the cache data to save. If <see langword="null"/>, an empty cache will be
    /// saved.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task SaveCacheAsync(IDictionary<string, object>? cache = null)
    {
        cache ??= _inMemoryCache.ToDictionary();
        var json = JsonSerializer.Serialize(cache);
        await _cacheBlobClient.PutAsync(_bloblCacheKey, System.Text.Encoding.UTF8.GetBytes(json));
    }
}