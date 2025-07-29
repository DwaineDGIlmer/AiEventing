namespace Core.Contracts;

/// <summary>
///  RAG (Retrieval-Augmented Generation) embedding service interface.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates a numerical embedding for the specified text.
    /// </summary>
    /// <param name="text">The input text for which the embedding is generated. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an array of floating-point
    /// numbers representing the embedding of the input text.</returns>
    Task<float[]> GetEmbeddingAsync(string text);
}
