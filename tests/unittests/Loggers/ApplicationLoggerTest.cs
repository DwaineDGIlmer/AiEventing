using Core.Configuration;
using Core.Services;
using Loggers.Application;
using Microsoft.Extensions.Logging;

namespace UnitTests.Loggers;

public class ApplicationLoggerTest : UnitTestsBase
{
    [Fact]
    public async Task Log_HandlesExceptionInFaultAnalysis_Gracefully()
    {
        var settings = new AiEventSettings { MinLogLevel = LogLevel.Information };
        var publisherMock = new MockPublisher();
        var client = GetFaultAnalysisServiceClient(new Exception("Network error"));
        var faultAnalysisService = new FaultAnalysisService(client, "chatgpt", "u123456", "http://work.com");
        var logger = new ApplicationLogger(
            "Test",
            settings,
            TestLogEventFactory,
            publisherMock,
            faultAnalysisService);

        var ex = new InvalidOperationException("fail");

        logger.Log(LogLevel.Information, new EventId(2, "Event"), "state", ex, (s, e) => $"Message: {s}");

        // Wait for the background task to complete
        await Task.Delay(2000);

        // Should write the main log and the error event for fault analysis failure
        Assert.True(publisherMock.Contains("Message: state"));
        Assert.True(publisherMock.Contains("Exception during fault analysis"));
    }

    private static HttpClient GetFaultAnalysisServiceClient(Exception exception)
    {
        return GetMockHttpClient(exception);
    }
}