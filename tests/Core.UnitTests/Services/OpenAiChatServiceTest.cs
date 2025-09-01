using Core.Configuration;
using Core.Contracts;
using Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OpenAI;

namespace Core.UnitTests.Services;

public sealed class OpenAiChatServiceTest
{
    private readonly Mock<OpenAIClient> _mockOpenAiClient;
    private readonly Mock<IOptions<OpenAiSettings>> _mockOptions;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ILogger<OpenAiChatService>> _mockLogger;
    private readonly OpenAiSettings _settings;

    public OpenAiChatServiceTest()
    {
        _mockOpenAiClient = new Mock<OpenAIClient>();
        _mockOptions = new Mock<IOptions<OpenAiSettings>>();
        _mockCacheService = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<OpenAiChatService>>();
        _settings = new OpenAiSettings { Model = "gpt-3.5-turbo" };
        _mockOptions.Setup(o => o.Value).Returns(_settings);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenClientIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OpenAiChatService(null!, _mockOptions.Object, _mockCacheService.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOptionsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OpenAiChatService(_mockOpenAiClient.Object, null!, _mockCacheService.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenCacheServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OpenAiChatService(_mockOpenAiClient.Object, _mockOptions.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OpenAiChatService(_mockOpenAiClient.Object, _mockOptions.Object, _mockCacheService.Object, null!));
    }

    [Fact]
    public async Task GetChatCompletion_ReturnsNewInstance_WhenMessagesAreEmpty()
    {
        var service = new OpenAiChatService(_mockOpenAiClient.Object, _mockOptions.Object, _mockCacheService.Object, _mockLogger.Object);

        var result = await service.GetChatCompletion<DummyResponse>("", "");

        Assert.NotNull(result);
        Assert.IsType<DummyResponse>(result);
    }

    [Fact]
    public async Task GetChatCompletion_ReturnsCachedItem_IfExists()
    {
        var cached = new DummyResponse { Value = "cached" };
        _mockCacheService.Setup(c => c.TryGetAsync<DummyResponse>(It.IsAny<string>()))
            .ReturnsAsync(cached);

        var service = new OpenAiChatService(_mockOpenAiClient.Object, _mockOptions.Object, _mockCacheService.Object, _mockLogger.Object);

        var result = await service.GetChatCompletion<DummyResponse>("system", "user");

        Assert.Equal("cached", result.Value);
    }

    // Dummy response class for testing
    public sealed class DummyResponse
    {
        public string? Value { get; set; }
    }
}