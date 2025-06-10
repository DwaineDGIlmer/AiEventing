using Core.Configuration;
using Core.Contracts;
using Core.Models;
using Core.Serializers;
using Core.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.UnitTests.Services;

public class FaultAnalysisServiceTest : UnitTestsBase
{
    private readonly IList<OpenAiMessage> _messages = [new() { Role = "user", Content = "Test" }];
    private readonly AiEventSettings _settings = new()
    {
        MinLogLevel = LogLevel.Information,
        OpenAiModel = "gpt-4o",
        OpenAiApiKey = "test-key",
        OpenAiApiUrl = "http://api.openai.com/v1/chat/completions",
        OpenAiClient = "OpenAiClient",
        RcaServiceClient = "RcaServiceClient",
        RcaServiceApiKey = "test-rca-key",
        RcaServiceUrl = "http://rca.service/api"
    };

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
    public void Constructor_ThrowsOnNullArguments()
    {
        var httpClientFactory = new Mock<IHttpClientFactory>().Object;
        var settings = _settings;

        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(null!, settings));
        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(httpClientFactory, null!));
    }

    [Fact]
    public async Task AnalyzeFaultAsync_LogEvent_SendsRequestAndReturnsTrue()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        var httpClient = CreateMockHttpClient(httpResponse);
        var factory = CreateMockHttpClientFactory(httpClient, _settings.RcaServiceClient);

        var service = new FaultAnalysisService(factory, _settings);

        var logEvent = new Mock<ILogEvent>().Object;
        var result = await service.AnalyzeFaultAsync(logEvent);

        Assert.True(result);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_LogEvent_ThrowsOnNull()
    {
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var factory = CreateMockHttpClientFactory(httpClient, _settings.RcaServiceClient);

        var service = new FaultAnalysisService(factory, _settings);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AnalyzeFaultAsync((ILogEvent)null!));
    }

    [Fact]
    public async Task AnalyzeFaultAsync_LogEvent_RcaDisabled_ReturnsFalse()
    {
        var settings = _settings;
        settings.RcaServiceEnabled = false;
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var factory = CreateMockHttpClientFactory(httpClient, settings.RcaServiceClient);

        var service = new FaultAnalysisService(factory, settings);

        var logEvent = new Mock<ILogEvent>().Object;
        var result = await service.AnalyzeFaultAsync(logEvent);

        Assert.False(result);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_String_SendsRequestAndReturnsResponse()
    {
        var expectedResponse = new OpenAiChatResponse { Id = "abc123" };
        var responseJson = JsonConvertService.Instance!.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };
        var httpClient = CreateMockHttpClient(httpResponse);
        var factory = CreateMockHttpClientFactory(httpClient, _settings.OpenAiClient);

        var service = new FaultAnalysisService(factory, _settings);

        var result = await service.AnalyzeFaultAsync(_messages);

        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_Exception_SendsRequestAndReturnsResponse()
    {
        var expectedResponse = new OpenAiChatResponse { Id = "xyz789" };
        var responseJson = JsonConvertService.Instance!.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };
        var httpClient = CreateMockHttpClient(httpResponse);
        var factory = CreateMockHttpClientFactory(httpClient, _settings.OpenAiClient);

        var service = new FaultAnalysisService(factory, _settings);
        var result = await service.AnalyzeFaultAsync(_messages);

        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_ThrowsOnNull_Response()
    {
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var factory = CreateMockHttpClientFactory(httpClient, _settings.OpenAiClient);
        var service = new FaultAnalysisService(factory, _settings);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AnalyzeFaultAsync(_messages));
    }

    [Fact]
    public async Task AnalyzeFaultAsync_ThrowsOnNullMessages()
    {
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var factory = CreateMockHttpClientFactory(httpClient, _settings.OpenAiClient);
        var service = new FaultAnalysisService(factory, _settings);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AnalyzeFaultAsync((IList<OpenAiMessage>)null!));
    }
}