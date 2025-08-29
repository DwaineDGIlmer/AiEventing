using Core.Constants;

namespace Core.Configuration;

/// <summary>
/// Represents the configuration settings for cache storage, including the account URL and container name.
/// </summary>
/// <remarks>This class is used to configure the connection details for blob storage, such as the account
/// URL and the container name. These settings are typically required for accessing and managing cached data in a
/// blob storage service.</remarks>
public class MemoryCacheSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the CacheLoader should be used for loading cache data.
    /// </summary>
    public bool UseCacheLoader { get; set; } = false;

    /// <summary>
    /// Gets or sets the AccountUrl for blob storage.
    /// </summary>
    public string? AccountUrl { get; set; } = null;

    /// <summary>
    /// Gats or sets the Container name used.
    /// </summary>
    public string Container { get; set; } = Defaults.Container;

    /// <summary>
    /// Gets or sets the name of the blob associated with the cache key.
    /// </summary>
    public string BlobName { get; set; } = Defaults.BlobName;

    /// <summary>
    /// Gets or sets the cache key used to uniquely identify cached items.
    /// </summary>
    public string CacheKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the prefix used to identify or categorize related items.
    /// </summary>
    public string Prefix { get; set; } = Defaults.Prefix;

    /// <summary>
    /// Gets or sets the expiration time, in minutes, for the associated resource or operation.
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 10;

    /// <summary>
    /// Gets or sets the number of minutes until a due item expires.
    /// </summary>
    public int DueExpirationInMinutes { get; set; } = 5;
}
