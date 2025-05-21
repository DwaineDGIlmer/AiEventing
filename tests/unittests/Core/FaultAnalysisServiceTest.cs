using Core.Models;
using Core.Serializers;
using Core.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnitTests.Core;

public class FaultAnalysisServiceTest : UnitTestsBase
{
    private readonly string _model = "test-model";
    private readonly string _apiKey = "test-api-key";
    private readonly string _apiUrl = "https://api.example.com/v1/chat/completions";
    private readonly IList<Message> _messages = [new() { Role = "user", Content = "Test" }];

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

    private static HttpClient CreateMockHttpClient(HttpResponseMessage responseMessage)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public void Constructor_ThrowsOnNullArguments()
    {
        var httpClient = new HttpClient();
        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(null!, _model, _apiKey, _apiUrl));
        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(httpClient, null!, _apiKey, _apiUrl));
        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(httpClient, _model, null!, _apiUrl));
        Assert.Throws<ArgumentNullException>(() => new FaultAnalysisService(httpClient, _model, _apiKey, null!));
    }

    [Fact]
    public async Task AnalyzeFaultAsync_String_SendsRequestAndReturnsResponse()
    {
        var expectedResponse = new ChatCompletionResponse { Id = "abc123" };
        var responseJson = JsonConvertService.Instance!.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };
        var httpClient = CreateMockHttpClient(httpResponse);

        var service = new FaultAnalysisService(httpClient, _model, _apiKey, _apiUrl);

        var result = await service.AnalyzeFaultAsync(_messages);

        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_Exception_SendsRequestAndReturnsResponse()
    {
        var expectedResponse = new ChatCompletionResponse { Id = "xyz789" };
        var responseJson = JsonConvertService.Instance!.Serialize(expectedResponse);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };
        var httpClient = CreateMockHttpClient(httpResponse);

        var service = new FaultAnalysisService(httpClient, _model, _apiKey, _apiUrl);
        var result = await service.AnalyzeFaultAsync(_messages);

        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }

    [Fact]
    public async Task AnalyzeFaultAsync_ThrowsOnNull_Response()
    {
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var service = new FaultAnalysisService(httpClient, _model, _apiKey, _apiUrl);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AnalyzeFaultAsync(_messages));
    }

    [Fact]
    public async Task AnalyzeFaultAsync_ThrowsOnNullMessages()
    {
        var httpClient = CreateMockHttpClient(new HttpResponseMessage(HttpStatusCode.OK));
        var service = new FaultAnalysisService(httpClient, _model, _apiKey, _apiUrl);

        await Assert.ThrowsAsync<ArgumentNullException>(() => service.AnalyzeFaultAsync(null!));
    }
}