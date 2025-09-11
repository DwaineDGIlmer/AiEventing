using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using Core.Helpers;
using System.Collections.Concurrent;

namespace Core.Caching;

/// <summary>
/// Provides a file-based implementation of the <see cref="ICacheService"/> interface for caching data.
/// </summary>
/// <remarks>This service stores cached data as JSON-serialized files in a specified directory.  It
/// supports basic cache operations such as retrieving, adding, and removing items. Note that this implementation
/// does not enforce expiration policies, even if an expiration time is provided.</remarks>
public partial class FileCacheService : ICacheService
{
    private readonly Task? _cleanUpTask = null;
    private readonly string _cacheDirectory;
    private readonly bool _enabled;
    private readonly ILogger<FileCacheService> _logger;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentDictionary<string, FileCacheEntry> _cachedEntries = [];
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

  
    /// <summary>
    /// Initializes a new instance of the <see cref="FileCacheService"/> class, which provides caching functionality for
    /// files with automatic cleanup of expired cache entries.
    /// </summary>
    /// <remarks>If the cache directory specified in the options is null, empty, or contains only whitespace,
    /// a default directory is used under the local application data folder. If the specified directory is not rooted,
    /// it is resolved relative to the system's temporary directory.  When caching is enabled, the service initializes a
    /// background cleanup task that periodically removes expired cache files from the cache directory. The cleanup task
    /// runs every minute as a long-running task.</remarks>
    /// <param name="logger">The logger instance used to log diagnostic and operational messages. This parameter cannot be null.</param>
    /// <param name="options">The configuration options for the caching service, including settings such as whether caching is enabled and the
    /// location of the cache directory. This parameter cannot be null.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null or if <paramref name="options"/> is null.</exception>
    public FileCacheService(
        ILogger<FileCacheService> logger,
        IOptions<AiEventSettings> options)
    {
        options.IsNullThrow(nameof(options), "Options cannot be null.");
        var enabled = options.Value.EnableCaching;
        var cacheDirectory = options.Value.CacheLocation;
        if (string.IsNullOrWhiteSpace(cacheDirectory))
        {
            cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cache");
        }
        else if (!Path.IsPathRooted(cacheDirectory))
        {
            cacheDirectory = Path.Combine(Path.GetTempPath(), cacheDirectory);
        }

        _enabled = enabled;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        _cacheDirectory = string.IsNullOrEmpty(cacheDirectory) || !Path.IsPathRooted(cacheDirectory)
            ? Path.GetTempPath()
            : cacheDirectory;

        Directory.CreateDirectory(_cacheDirectory);

        if (_enabled && _cleanUpTask is null)
        {
            var files = Directory.GetFiles(_cacheDirectory, "*.cache");
            foreach (var file in files)
            {
                _cachedEntries.TryAdd(file, new FileCacheEntry(file, null));
            }

            _logger.LogInformation("Starting cleanup task for expired cache files in {CacheDirectory}", _cacheDirectory);
            _cleanUpTask = new Task(async () =>
            {
                while (_enabled)
                {
                    RemoveExpiredFiles(_cachedEntries, _logger);
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }, TaskCreationOptions.LongRunning);
            _cleanUpTask.Start();
        }
    }

    /// <summary>
    /// Asynchronously retrieves an object of the specified type from a file associated with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    /// <param name="key">The key used to identify the file containing the serialized object.</param>
    /// <returns>The deserialized object of type <typeparamref name="T"/> if the file exists and can be read;  otherwise,
    /// <see langword="default"/> for the type <typeparamref name="T"/>.</returns>
    /// <exception cref="JsonException">Occurs when json serialization fails.</exception>
    public async Task<T?> TryGetAsync<T>(string key)
    {
        if (!_enabled)
        {
            return default;
        }

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError("Key cannot be null or empty.");
            return default;
        }

        // Initialize the JSON string to be read from the file
        string json = string.Empty;
        string path = GetFilePath(key, _cacheDirectory);

        // Use a semaphore to ensure thread safety when accessing the file system
        await _semaphore.WaitAsync();
        try
        {
            if (!File.Exists(path))
                return default;

            json = await File.ReadAllTextAsync(path);
            if (string.IsNullOrEmpty(json))
                return default;

            // Try direct deserialization first
            _logger.LogInformation("Found in cache, attempting to deserialize JSON for type {Type} from file {Path}", typeof(T).Name, path);
            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (JsonException ex)
        {
            _logger.LogInformation("Initial JSON deserialization failed for type {Type}: {Message}. Attempting sanitization.", typeof(T).Name, ex.Message);

            // Attempt sanitization
            var sanitizedJson = SanitizeJsonString(json);
            try
            {
                return JsonSerializer.Deserialize<T>(sanitizedJson, _options);
            }
            catch (JsonException retryEx)
            {
                _logger.LogError("Sanitized JSON deserialization also failed for type {Type}: {Message}", typeof(T).Name, retryEx.Message);
            }
        }
        finally
        {
            _semaphore.Release();
        }

        if (File.Exists(path))
        {
            Remove(path);
        }
        return default;
    }

    /// <summary>
    /// Asynchronously sets a value in the storage with the specified key.
    /// </summary>
    /// <remarks>The value is serialized to JSON and written to a file. The file path is determined by
    /// the key. Note that the current implementation does not enforce expiration.</remarks>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <param name="key">The unique key associated with the value. Cannot be null or empty.</param>
    /// <param name="value">The value to store. Cannot be null.</param>
    /// <param name="absoluteExpiration">An optional expiration time for the stored value. This parameter is currently ignored in the implementation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task CreateEntryAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        if (!_enabled)
        {
            return;
        }

        if (string.IsNullOrEmpty(key) || value is null)
        {
            _logger.LogError("Key or value cannot be null. Key: {Key}, Value: {Value}", key, value);
            return;
        }

        try
        {
            var path = GetFilePath(key, _cacheDirectory);
            var json = JsonSerializer.Serialize(value);
            if (!string.IsNullOrEmpty(json))
            {
                await File.WriteAllTextAsync(path, json);
                _cachedEntries.TryAdd(path, new FileCacheEntry(path, absoluteExpiration));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error creating cache entry for key {Key}: {Message}", key, ex.Message);
        }
    }

    /// <summary>
    /// Asynchronously removes the item associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous remove operation.</returns>
    public Task RemoveAsync(string key)
    {
        if (string.IsNullOrEmpty(key) || !_enabled)
        {
            return Task.CompletedTask;
        }

        return Task.Run(() =>
        {
            var path = GetFilePath(key, _cacheDirectory);
            Remove(path);
            _ = _cachedEntries.TryRemove(path, out var entry);
        });
    }

    /// <summary>
    /// Removes the specified file from the file system if the operation is enabled.
    /// </summary>
    /// <param name="filePath">The path of the file to be removed. Must not be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filePath"/> is null or empty.</exception>
    public static void Remove(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("Key and cache directory cannot be null or empty.");
        }

        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    /// <summary>
    /// Removes expired files from the cache directory.
    /// </summary>
    /// <remarks>This method iterates over the provided cached entries and removes those that have expired
    /// based on their absolute expiration time. Any errors encountered during the removal of files are
    /// logged using the provided <paramref name="logger"/>.</remarks>
    /// <param name="cachedEntries">A collection of cached file entries to be checked for expiration.</param>
    /// <param name="logger">The logger used to log errors during the removal process.</param>
    public static void RemoveExpiredFiles(
        ConcurrentDictionary<string, FileCacheEntry> cachedEntries,
        ILogger logger)
    {
        foreach (var entry in cachedEntries.ToList())
        {
            try
            {
                if (entry.Value.AbsoluteExpiration < DateTime.UtcNow)
                {
                    Remove(entry.Value.CacheEntry);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error removing expired files: {Message}", ex.Message);
            }
        }
    }

    /// <summary>
    /// Constructs a file path by combining the specified cache directory and a key.
    /// </summary>
    /// <param name="key">The unique identifier for the cache file. This will be used as the file name with a ".cache" extension.</param>
    /// <param name="cacheDirectory">The directory where the cache files are stored. This must be a valid directory path.</param>
    /// <returns>The full file path as a string, combining the cache directory and the key with a ".cache" extension.</returns>
    public static string GetFilePath(string key, string cacheDirectory)
    {
        key.IsNullThrow(nameof(key), "Key cannot be null or empty.");
        cacheDirectory.IsNullThrow(nameof(cacheDirectory), "Cache directory cannot be null or empty.");
        if (cacheDirectory.Any(c => Path.GetInvalidPathChars().Contains(c)))
        {
            throw new ArgumentException("Cache directory contains invalid characters.", nameof(cacheDirectory));
        }

        if (key.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            return Path.Combine(cacheDirectory, $"{key.FileSystemName()}.cache");
        }
        return Path.Combine(cacheDirectory, $"{key}.cache");
    }

    /// <summary>
    /// Sanitizes a JSON string to ensure it is well-formed and free of invalid characters.
    /// </summary>
    /// <remarks>This method attempts to parse the input JSON string to verify its validity. If the input is
    /// invalid, it applies a sanitization process to correct common issues and then verifies the result. The returned
    /// string is trimmed of any leading or trailing whitespace.</remarks>
    /// <param name="jsonString">The JSON string to sanitize. This parameter cannot be null or whitespace.</param>
    /// <returns>A sanitized version of the input JSON string that is well-formed. If the input is already valid, it returns the
    /// original string.</returns>
    private static string SanitizeJsonString(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return jsonString;

        try
        {
            JsonDocument.Parse(jsonString);
            return jsonString;
        }
        catch { }

        var sanitized = CoreRegex.SanitizeJson(jsonString);
        try
        {
            JsonDocument.Parse(sanitized);
            return sanitized.Trim();
        }
        catch
        {
            return sanitized.Trim();
        }
    }
}

/// <summary>
/// Represents a cache entry for a file, including creation time and optional expiration settings.
/// </summary>
/// <remarks>This class provides a mechanism to store cache entries with a specified absolute expiration time. If
/// no expiration time is provided, a default expiration of one day from the creation time is used.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="FileCacheEntry"/> class with an optional absolute expiration time.
/// </remarks>
/// <param name="cacheItem">Item to cache.</param>
/// <param name="absoluteExpiration">The absolute expiration time for the cache entry. If <see langword="null"/>, the entry does not have a fixed
/// expiration time.</param>
public sealed class FileCacheEntry(string cacheItem, TimeSpan? absoluteExpiration = null)
{
    private static readonly TimeSpan _defaultExpiration = TimeSpan.FromDays(1);

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the absolute expiration date and time for the cache entry.
    /// </summary>
    public DateTime AbsoluteExpiration { get; } = absoluteExpiration is not null ? DateTime.UtcNow.Add((TimeSpan)absoluteExpiration) : DateTime.UtcNow.Add(_defaultExpiration);

    /// <summary>
    /// Gets or sets the cache entry value.
    /// </summary>
    public string CacheEntry { get; set; } = cacheItem ?? throw new ArgumentNullException(nameof(cacheItem), "Cache item cannot be null.");
}
