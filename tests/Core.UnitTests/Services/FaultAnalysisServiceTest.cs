using Core.Configuration;
using Core.Contracts;
using Core.Models;
using Core.Serializers;
using Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.UnitTests.Services;

public class FaultAnalysisServiceTest : UnitTestsBase
{
    private readonly IList<OpenAiMessage> _messages = [new() { Role = "user", Content = "Test" }];
    private readonly IOptions<AiEventSettings> _aiEventSettings = Options.Create(new AiEventSettings()
    {
        MinLogLevel = LogLevel.Information,
        RcaServiceClient = "RcaServiceClient",
        RcaServiceApiKey = "test-rca-key",
        RcaServiceUrl = "http://rca.service/api"
    });

    public FaultAnalysisServiceTest()
    {
        if (!JsonConvertService.IsInitialized)
        {
            JsonConvertService.Initialize(new JsonSerializerOptions()
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            });
        }
    }


    [Fact]
    public void Constructor_ThrowsOnNullArguments_AiEventSettings_Missing()
    {
        var chatService = new Mock<IOpenAiChatService>().Object;
        var settings = _aiEventSettings;

        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(null!, settings));
    }

    [Fact]
    public void FaultServiceEnabled_ReturnsTrue_WhenEnabledInSettings()
    {
        var chatService = new Mock<IOpenAiChatService>().Object;
        var settings = Options.Create(new AiEventSettings
        {
            FaultServiceEnabled = true,
        });

        var service = new FaultAnalysisService(chatService, settings);

        Assert.True(service.FaultServiceEnabled);
    }

    [Fact]
    public void FaultServiceEnabled_ReturnsFalse_WhenDisabledInSettings()
    {
        var chatService = new Mock<IOpenAiChatService>().Object;
        var settings = Options.Create(new AiEventSettings
        {
            FaultServiceEnabled = false,
        });

        var service = new FaultAnalysisService(chatService, settings);

        Assert.False(service.FaultServiceEnabled);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_ReturnsFalse_WhenRcaServiceDisabled()
    {
        var chatService = new Mock<IOpenAiChatService>().Object;
        var settings = Options.Create(new AiEventSettings
        {
            FaultServiceEnabled = false,
        });

        var service = new FaultAnalysisService(chatService, settings);
        var logEvent = new Mock<ILogEvent>().Object;

        var result = await service.AnalyzeFaultAsync(logEvent);

        Assert.False(result);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_ThrowsOnNullFault()
    {
        var chatService = new Mock<IOpenAiChatService>().Object;
        var settings = _aiEventSettings;
        var service = new FaultAnalysisService(chatService, settings);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AnalyzeFaultAsync(null!));
    }
}