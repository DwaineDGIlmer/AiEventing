using Core.Enums;

namespace Core.UnitTests.Constants;

public sealed class OpenAIEmbeddingDimensionsTest
{
    [Theory]
    [InlineData(OpenAIEmbeddingDimensions.Embedding3Small, 1536)]
    [InlineData(OpenAIEmbeddingDimensions.TextEmbedding3Large, 3072)]
    [InlineData(OpenAIEmbeddingDimensions.CohereEmbed, 1024)]
    [InlineData(OpenAIEmbeddingDimensions.SbertMiniLM, 384)]
    [InlineData(OpenAIEmbeddingDimensions.SbertMpnetBase, 768)]
    public void EnumValue_ShouldMatchExpectedDimension(OpenAIEmbeddingDimensions dimension, int expected)
    {
        Assert.Equal(expected, (int)dimension);
    }

    [Fact]
    public void Enum_ShouldContainAllExpectedValues()
    {
        var values = (OpenAIEmbeddingDimensions[])System.Enum.GetValues(typeof(OpenAIEmbeddingDimensions));
        Assert.Contains(OpenAIEmbeddingDimensions.Embedding3Small, values);
        Assert.Contains(OpenAIEmbeddingDimensions.TextEmbedding3Large, values);
        Assert.Contains(OpenAIEmbeddingDimensions.CohereEmbed, values);
        Assert.Contains(OpenAIEmbeddingDimensions.SbertMiniLM, values);
        Assert.Contains(OpenAIEmbeddingDimensions.SbertMpnetBase, values);
        Assert.Equal(5, values.Length);
    }
}