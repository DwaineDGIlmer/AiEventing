using Core.Constants;
using Core.Contracts;
using Core.Helpers;
using Core.Models;
using OpenAI;

namespace Core.Services;

/// <summary>
/// Provides functionality to generate text embeddings using the Ollama API.
/// </summary>
/// <remarks>This service communicates with an embedding API endpoint to generate embeddings for a given
/// text input. The service is designed to work with a specific model, such as "llama3", and requires the API
/// endpoint URL to be provided during initialization.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="OpenAiEmbeddingService"/> class.
/// </remarks>
/// <remarks>This constructor creates a default instance of the <see
/// cref="OpenAiEmbeddingService"/> class. Use this constructor when no specific configuration or dependencies
/// are required.</remarks>
internal class OpenAiEmbeddingService(OpenAIClient openAiClient, ICacheService cacheService) : IEmbeddingService
{
    private readonly OpenAIClient _openAiApi = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient), "OpenAIClient cannot be null.");
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService), "CacheService cannot be null.");
    private readonly ulong _expectedDimension = OpenAIEmbeddingModels.GetDimension(DefaultConstants.DefaultEmbedding);

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var embeddingClient = _openAiApi.GetEmbeddingClient(DefaultConstants.DefaultEmbedding);
        var embeddingResult = await embeddingClient.GenerateEmbeddingsAsync([text]);
        var embedding = embeddingResult.Value.FirstOrDefault()
            ?? throw new Exception("Failed to get embedding from OpenAI.");
        return embedding.ToFloats().ToArray();
    }

    /// <inheritdoc/>
    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var cacheKey = GenerateCacheKey(text);
        var cached = await _cacheService.TryGetAsync<float[]>(cacheKey);
        if (cached != null) return cached;

        var embeddingArray = await GenerateEmbeddingAsync(text);
        ValidateEmbeddingDimensions(embeddingArray);

        await _cacheService.CreateEntryAsync(cacheKey, embeddingArray);
        return embeddingArray;
    }

    private void ValidateEmbeddingDimensions(float[] embeddingArray)
    {
        if (embeddingArray.Length != (int)_expectedDimension)
        {
            throw new Exception($"Embedding size mismatch for model '{DefaultConstants.DefaultEmbedding}': expected {_expectedDimension}, got {embeddingArray.Length}");
        }
    }

    /// <summary>
    /// Generates a cache key for the given text using a hash function.
    /// Note: This does not handle hash collisions explicitly. If your cache backend has key length limits,
    /// consider truncating the hash or using a different hash function.
    /// </summary>
    /// <param name="text">The input text to hash.</param>
    /// <returns>A hashed string suitable for use as a cache key.</returns>
    private static string GenerateCacheKey(string text)
    {
        return CachingHelper.GenCacheKey(nameof(OpenAiEmbeddingService), text);
    }
}
