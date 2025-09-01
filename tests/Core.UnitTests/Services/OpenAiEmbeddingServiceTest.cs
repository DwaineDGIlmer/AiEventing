using Core.Contracts;
using Core.Extensions;
using Core.Services;
using Moq;
using OpenAI;

namespace Core.UnitTests.Services;

public sealed class OpenAiEmbeddingServiceTest
{
    private readonly Mock<OpenAIClient> _openAiClientMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly OpenAiEmbeddingService _service;

    public OpenAiEmbeddingServiceTest()
    {
        _openAiClientMock = new Mock<OpenAIClient>();
        _cacheServiceMock = new Mock<ICacheService>();
        _service = new OpenAiEmbeddingService(_openAiClientMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task GetEmbeddingAsync_ReturnsCachedEmbedding_WhenPresent()
    {
        var text = "test";
        var cacheKey = text.GenHashString();
        var cachedEmbedding = new float[] { 1.0f, 2.0f, 3.0f };

        _cacheServiceMock.Setup(c => c.TryGetAsync<float[]>(It.IsAny<string>()))
            .ReturnsAsync(cachedEmbedding);

        var result = await _service.GetEmbeddingAsync(text);

        Assert.Equal(cachedEmbedding, result);
        _cacheServiceMock.Verify(c => c.TryGetAsync<float[]>(It.IsAny<string>()), Times.Once);
        _openAiClientMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_ThrowsException_WhenTextIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GenerateEmbeddingAsync(null!));
    }

    [Fact]
    public async Task GetEmbeddingAsync_ThrowsException_WhenTextIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetEmbeddingAsync(null!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOpenAiClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenAiEmbeddingService(null!, _cacheServiceMock.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenCacheServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenAiEmbeddingService(_openAiClientMock.Object, null!));
    }
}