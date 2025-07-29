using Core.Enums;
using Core.Models;

namespace Core.UnitTests.Models;

public class OpenAIEmbeddingModelsTest
{
    [Theory]
    [InlineData(OpenAIEmbeddingModels.Embedding3Small, (ulong)OpenAIEmbeddingDimensions.Embedding3Small)]
    [InlineData(OpenAIEmbeddingModels.TextEmbedding3Small, (ulong)OpenAIEmbeddingDimensions.Embedding3Small)]
    [InlineData(OpenAIEmbeddingModels.TextEmbedding3Large, (ulong)OpenAIEmbeddingDimensions.TextEmbedding3Large)]
    [InlineData(OpenAIEmbeddingModels.CohereEmbed, (ulong)OpenAIEmbeddingDimensions.CohereEmbed)]
    [InlineData(OpenAIEmbeddingModels.SbertMiniLM, (ulong)OpenAIEmbeddingDimensions.SbertMiniLM)]
    [InlineData(OpenAIEmbeddingModels.SbertMpnetBase, (ulong)OpenAIEmbeddingDimensions.SbertMpnetBase)]
    public void GetDimension_ReturnsCorrectDimension_ForKnownModels(string model, ulong expectedDimension)
    {
        var dimension = OpenAIEmbeddingModels.GetDimension(model);
        Assert.Equal(expectedDimension, dimension);
    }

    [Fact]
    public void GetDimension_ThrowsArgumentException_ForUnknownModel()
    {
        var unknownModel = "unknown-model";
        var ex = Assert.Throws<ArgumentException>(() => OpenAIEmbeddingModels.GetDimension(unknownModel));
        Assert.Contains("Unknown embedding model", ex.Message);
    }

    [Fact]
    public void ModelDimensions_ContainsAllExpectedModels()
    {
        Assert.True(OpenAIEmbeddingModels.ModelDimensions.ContainsKey(OpenAIEmbeddingModels.Embedding3Small));
        Assert.True(OpenAIEmbeddingModels.ModelDimensions.ContainsKey(OpenAIEmbeddingModels.TextEmbedding3Small));
        Assert.True(OpenAIEmbeddingModels.ModelDimensions.ContainsKey(OpenAIEmbeddingModels.TextEmbedding3Large));
        Assert.True(OpenAIEmbeddingModels.ModelDimensions.ContainsKey(OpenAIEmbeddingModels.CohereEmbed));
        Assert.True(OpenAIEmbeddingModels.ModelDimensions.ContainsKey(OpenAIEmbeddingModels.SbertMiniLM));
        Assert.True(OpenAIEmbeddingModels.ModelDimensions.ContainsKey(OpenAIEmbeddingModels.SbertMpnetBase));
    }
}