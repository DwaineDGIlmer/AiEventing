namespace Core.Contracts
{
    /// <summary>
    /// Provides a way to load and save cached items into warm storage asynchronously.
    /// </summary>
    /// <remarks>This interface provides an abstraction for managing cache data, including saving the data to
    /// a storage medium (e.g., Azure Blob Storage) and retrieving it as key-value pairs. Implementations of this
    /// interface are expected to handle serialization and deserialization of the cache data.</remarks>
    public interface ICacheLoader
    {
        /// <summary>
        /// Asynchronously stores a value in the cache with the specified key and optional expiration time.
        /// </summary>
        /// <typeparam name="T">The type of the value to store in the cache.</typeparam>
        /// <param name="key">The unique key used to identify the cached value. Cannot be null or empty.</param>
        /// <param name="value">The value to store in the cache. Cannot be null.</param>
        /// <param name="absoluteExpiration">An optional expiration time for the cached value. If specified, the value will expire and be removed  from
        /// the cache after the given duration. If null, the value will not have an explicit expiration.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task PutAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);

        /// <summary>
        /// Asynchronously loads or gets a warm cache of key-value pairs.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary where the keys are
        /// strings and the values are objects, representing the cached data. If no data is available, the dictionary
        /// will be empty.</returns>
        Task<IDictionary<string, object>> LoadCacheAsync();

        /// <summary>
        /// Asynchronously saves the provided cache data.
        /// </summary>
        /// <remarks>This method persists the provided cache data asynchronously. The exact storage
        /// mechanism is implementation-dependent. Ensure that the dictionary keys are unique and meaningful for
        /// retrieval purposes.</remarks>
        /// <param name="cache">A dictionary containing the cache data to save, where the key is a string identifier and the value is the
        /// associated object. The dictionary cannot be null, and keys must be unique.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task SaveCacheAsync(IDictionary<string, object>? cache);
    }
}
