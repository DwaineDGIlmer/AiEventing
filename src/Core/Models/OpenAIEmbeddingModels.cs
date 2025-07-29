using Core.Enums;

namespace Core.Models
{
    /// <summary>
    /// Provides a collection of OpenAI embedding models and their associated dimensionalities.
    /// </summary>
    /// <remarks>This class contains a predefined set of embedding models and their corresponding dimensions, 
    /// which can be accessed through the <see cref="ModelDimensions"/> dictionary. It also provides  a method to
    /// retrieve the dimensionality of a specific model.</remarks>
    public static class OpenAIEmbeddingModels
    {
        /// <summary>
        /// Represents the identifier for the "text-embedding-ada-002" embedding model.
        /// </summary>
        /// <remarks>This constant can be used to specify the "text-embedding-ada-002" model in operations
        /// that require a model identifier. Ensure that the target system or API supports this specific model before
        /// using it.</remarks>
        public const string Embedding3Small = "text-embedding-ada-002";

        /// <summary>
        /// Represents the identifier for the "text-embedding-3-large" embedding model.
        /// </summary>
        /// <remarks>This constant can be used to specify the "text-embedding-3-large" model in operations
        /// that require a model identifier. Ensure that the target system or API supports this specific model before
        /// using it.</remarks>
        public const string TextEmbedding3Large = "text-embedding-3-large";

        /// <summary>
        /// Represents the identifier for the Cohere embedding model.
        /// </summary>
        /// <remarks>This constant can be used to specify or reference the Cohere embedding model in
        /// operations that require a model identifier.</remarks>
        public const string CohereEmbed = "Cohere";

        /// <summary>
        /// Represents the identifier for the "all-MiniLM-L6-v2" model used in sentence embeddings.
        /// </summary>
        /// <remarks>This constant is typically used to specify the "all-MiniLM-L6-v2" model in
        /// applications that require sentence embeddings or semantic similarity calculations. The model is part of the
        /// SBERT (Sentence-BERT) family and is optimized for performance and accuracy in natural language processing
        /// tasks.</remarks>
        public const string SbertMiniLM = "all-MiniLM-L6-v2";

        /// <summary>
        /// Represents the identifier for the "all-mpnet-base-v2" Sentence-BERT model.
        /// </summary>
        /// <remarks>This constant can be used to specify the "all-mpnet-base-v2" model in applications
        /// that require a predefined model identifier for natural language processing tasks.</remarks>
        public const string SbertMpnetBase = "all-mpnet-base-v2";

        /// <summary>
        /// Represents the identifier for the "text-embedding-3-small" model.
        /// </summary>
        /// <remarks>This constant can be used to specify the "text-embedding-3-small" model in operations
        /// that require a model identifier.</remarks>
        public const string TextEmbedding3Small = "text-embedding-3-small";

        /// <summary>
        /// Provides a mapping of model names to their corresponding embedding dimensions.
        /// </summary>
        /// <remarks>This dictionary contains predefined entries for various embedding models, where the
        /// key is the model name and the value is the embedding dimension associated with that model. The dimensions
        /// are derived from the <see cref="OpenAIEmbeddingDimensions"/> enumeration.  Use this dictionary to look up
        /// the embedding dimensions for supported models when working with text embedding tasks.</remarks>
        public static readonly Dictionary<string, ulong> ModelDimensions = new()
        {
            { Embedding3Small, (ulong)OpenAIEmbeddingDimensions.Embedding3Small },
            { TextEmbedding3Small, (ulong)OpenAIEmbeddingDimensions.Embedding3Small },
            { TextEmbedding3Large, (ulong)OpenAIEmbeddingDimensions.TextEmbedding3Large },
            { CohereEmbed, (ulong)OpenAIEmbeddingDimensions.CohereEmbed },
            { SbertMiniLM, (ulong)OpenAIEmbeddingDimensions.SbertMiniLM },
            { SbertMpnetBase, (ulong)OpenAIEmbeddingDimensions.SbertMpnetBase },
        };

        /// <summary>
        /// Retrieves the dimensionality of the specified embedding model.
        /// </summary>
        /// <param name="model">The name of the embedding model for which to retrieve the dimensionality. Cannot be null or empty.</param>
        /// <returns>The dimensionality of the specified embedding model.</returns>
        /// <exception cref="ArgumentException">Thrown if the specified <paramref name="model"/> is not recognized.</exception>
        public static ulong GetDimension(string model)
        {
            if (ModelDimensions.TryGetValue(model, out var dim))
            {
                return dim;
            }
            throw new ArgumentException($"Unknown embedding model: {model}");
        }
    }
}
