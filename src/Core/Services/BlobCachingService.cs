using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Core.Configuration;
using Core.Contracts;
using Core.Extensions;

namespace Core.Services;

/// <summary>
/// Blob-backed implementation for application cache with transparent gzip compression.
/// </summary>
public sealed class BlobCachingService : ICacheBlobClient
{
    private readonly BlobContainerClient _blobClient;
    private readonly string _blobName;
    private readonly string _prefix;

    /// <summary>
    /// Initializes the cache client using DI-provided BlobServiceClient and appsettings.
    /// </summary>
    public BlobCachingService(
        BlobServiceClient serviceClient,
        IOptions<MemoryCacheSettings> config)
    {
        config.IsNullThrow(nameof(config));
        serviceClient.IsNullThrow(nameof(serviceClient));

        _prefix = config.Value.Prefix;
        _blobName = config.Value.BlobName;
        _blobClient = serviceClient.GetBlobContainerClient(config.Value.Container);
        _blobClient.CreateIfNotExists(PublicAccessType.None);
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetAsync(string key, CancellationToken ct = default)
    {
        var blob = _blobClient.GetBlobClient(PathFor(key));
        if (!await blob.ExistsAsync(ct))
        {
            return null;
        }

        var resp = await blob.DownloadContentAsync(ct);
        var content = resp.Value.Content.ToArray();
        if (resp.Value.Details.ContentEncoding == "gzip")
        {
            using var input = new MemoryStream(content);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            await gzip.CopyToAsync(output, ct);
            return output.ToArray();
        }
        return content;
    }

    /// <inheritdoc/>
    public async Task<string> PutAsync(string key, byte[] data, string? ifMatchEtag = null, CancellationToken ct = default)
    {
        // Compress to reduce bandwidth and egress
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, leaveOpen: true))
            await gzip.WriteAsync(data, ct);
        output.Position = 0;

        var blob = _blobClient.GetBlobClient(PathFor(key));
        var headers = new BlobHttpHeaders
        {
            ContentType = "application/octet-stream",
            ContentEncoding = "gzip",
            CacheControl = "public, max-age=300" // browsers/proxies; adjust as needed
        };

        var conditions = new BlobRequestConditions();
        if (!string.IsNullOrEmpty(ifMatchEtag))
            conditions.IfMatch = new ETag(ifMatchEtag);

        var result = await blob.UploadAsync(output, new BlobUploadOptions
        {
            HttpHeaders = headers,
            Conditions = string.IsNullOrEmpty(ifMatchEtag) ? null : conditions,
            TransferOptions = new StorageTransferOptions
            {
                MaximumConcurrency = Environment.ProcessorCount,
                MaximumTransferSize = 4 * 1024 * 1024
            }
        }, ct);

        return result.Value.ETag.ToString();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string key, CancellationToken ct = default)
    {
        var blob = _blobClient.GetBlobClient(PathFor(key));
        await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, conditions: null, cancellationToken: ct);
    }

    /// <summary>Builds a normalized blob path with the configured prefix.</summary>
    private string PathFor(string key)
    {
        return $"{_prefix.TrimEnd('/')}/cache/{key}".TrimStart('/');
    }
}
