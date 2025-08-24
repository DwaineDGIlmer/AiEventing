namespace Core.Contracts;

/// <summary>
/// Provides read/write access to cache blobs with versioned keys and ETag-aware updates.
/// </summary>
public interface ICacheBlobClient
{
    /// <summary>
    /// Asynchronously retrieves the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose associated value is to be retrieved. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Defaults to <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value as a byte array if the key
    /// exists; otherwise, <see langword="null"/>.</returns>
    Task<byte[]?> GetAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously stores the specified data under the given key, with optional conditional update behavior.
    /// </summary>
    /// <remarks>If the specified key already exists, the data will be overwritten. If the <paramref
    /// name="ifMatchEtag"/>  parameter is provided and does not match the current ETag of the stored data, the
    /// operation will fail.</remarks>
    /// <param name="key">The unique identifier for the data to be stored. Cannot be null or empty.</param>
    /// <param name="data">The byte array representing the data to be stored. Cannot be null.</param>
    /// <param name="ifMatchEtag">An optional ETag value used for conditional updates. If specified, the operation will only succeed  if the
    /// current ETag of the stored data matches this value. Pass <see langword="null"/> to perform an  unconditional
    /// update.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the ETag of the stored data.</returns>
    Task<string> PutAsync(string key, byte[] data, string? ifMatchEtag = null, CancellationToken ct = default);

    /// <summary>
    /// Deletes the item associated with the specified key asynchronously.
    /// </summary>
    /// <remarks>This method performs the delete operation asynchronously. If the specified key does not
    /// exist, a <see cref="KeyNotFoundException"/> is thrown. Ensure that the key is valid and corresponds to an
    /// existing item.</remarks>
    /// <param name="key">The unique identifier of the item to delete. Cannot be null or empty.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Optional.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(string key, CancellationToken ct = default);
}
