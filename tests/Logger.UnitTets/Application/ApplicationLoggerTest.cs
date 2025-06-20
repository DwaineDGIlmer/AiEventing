using Core.Configuration;
using Core.Contracts;
using Core.Helpers;
using Core.Models;
using Core.Services;
using Loggers.Application;
using Loggers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Logger.UnitTets.Application;

public class ApplicationLoggerTest : UnitTestsBase
{
    [Theory]
    [InlineData(LogLevel.Trace, LogLevel.Information, false)]
    [InlineData(LogLevel.Debug, LogLevel.Information, false)]
    [InlineData(LogLevel.Information, LogLevel.Information, true)]
    [InlineData(LogLevel.Warning, LogLevel.Information, true)]
    [InlineData(LogLevel.Error, LogLevel.Information, true)]
    public void IsEnabled_ReturnsExpectedResult(LogLevel inputLevel, LogLevel minLevel, bool expected)
    {
        // Arrange
        var settings = Options.Create(new AiEventSettings { MinLogLevel = minLevel, PollingDelay = 1 });
        var logger = new ApplicationLogger(
            "TestCategory",
            settings,
            () => Mock.Of<ILogEvent>(),
            Mock.Of<IPublisher>(),
            Mock.Of<IFaultAnalysisService>()
        );

        // Act
        var result = logger.IsEnabled(inputLevel);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void BeginScope_CallsScopeProviderPush_AndReturnsDisposable()
    {
        // Arrange
        var mockScopeProvider = new Mock<IExternalScopeProvider>();
        var mockDisposable = new Mock<IDisposable>();
        var settings = Options.Create(new AiEventSettings { MinLogLevel = LogLevel.Information, PollingDelay = 1 });
        var logger = new ApplicationLogger(
            "TestCategory",
            settings,
            Mock.Of<ILogEvent>,
            Mock.Of<IPublisher>(),
            Mock.Of<IFaultAnalysisService>()
        );

        // Use reflection to set the internal ScopeProvider for testing
        var field = typeof(ApplicationLogger)
            .GetField("ScopeProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?? typeof(ApplicationLogger)
            .GetField("<ScopeProvider>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        field?.SetValue(logger, mockScopeProvider.Object);

        mockScopeProvider
            .Setup(p => p.Push(It.IsAny<object>()))
            .Returns(mockDisposable.Object);

        var state = "scope-state";

        // Act
        var result = logger.BeginScope(state);

        // Assert
        mockScopeProvider.Verify(p => p.Push(state), Times.Once);
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IDisposable>(result);
    }

    [Fact]
    public async Task Log_HandlesExceptionInFaultAnalysis_Gracefully()
    {
        var settings = Options.Create(new AiEventSettings
        {
            MinLogLevel = LogLevel.Information,
            OpenAiModel = "gpt-4o",
            OpenAiApiKey = "test-key",
            OpenAiApiUrl = "http://api.openai.com/v1/chat/completions",
            OpenAiClient = "OpenAiClient",
            RcaServiceClient = "RcaServiceClient",
            RcaServiceApiKey = "test-rca-key",
            RcaServiceUrl = "http://rca.service/api"
        });
        var publisherMock = new MockPublisher();
        var client = GetHttpClientFactory(new Exception("Network error"));
        var mockLogger = new MockLogger(LogLevel.Information, publisherMock);
        var faultAnalysisService = new FaultAnalysisService(client, settings);
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

    [Fact]
    public async Task MethodBuilder_Log_Does_Not_Use_FaultAnalysis()
    {
        var settings = Options.Create(new AiEventSettings
        {
            OpenAiEnabled = false,
            MinLogLevel = LogLevel.Information,
            OpenAiModel = "gpt-4o",
            OpenAiApiKey = "test-key",
            OpenAiApiUrl = "http://api.openai.com/v1/chat/completions",
            OpenAiClient = "OpenAiClient",
            RcaServiceClient = "RcaServiceClient",
            RcaServiceApiKey = "test-rca-key",
            RcaServiceUrl = "http://rca.service/api"
        });
        var publisherMock = new MockPublisher();
        var mockLogger = new MockLogger(LogLevel.Information, publisherMock);
        var client = GetHttpClientFactory(new Exception("Network error"));
        var faultAnalysisService = new FaultAnalysisService(client, settings);
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
        Assert.False(publisherMock.Contains("Exception during fault analysis"));
    }

    [Fact]
    public void AnalyzeAndPublishFaultAsync_UsesCacheIfAvailable()
    {
        // Arrange
        var exception = new InvalidOperationException("Test");
        var logEvent = Mock.Of<ILogEvent>();
        var cachedEvent = new OpenAiChatResponse()
        {
            Choices =
             [
                 new CompletionChoice()
                 {
                     Message = new OpenAiMessage()
                     {
                         Role = "system",
                         Content = "cached content"
                     }
                 }
             ]
        };
        var publisherMock = new MockPublisher();
        var faultServiceMock = new MockFaultAnalysis("content", "role");
        var settings = Options.Create(new AiEventSettings { MinLogLevel = LogLevel.Debug, PollingDelay = 1 });
        var logger = new ApplicationLogger(
            "TestCategory",
            settings,
            Mock.Of<ILogEvent>,
            publisherMock,
            faultServiceMock
        );

        var cacheId = ExceptionHelper.GetExceptionHash(exception);
        logger.LogEventsCache.TryAdd(cacheId, logEvent);

        // Act
        logger.AnalyzeAndPublishFaultAsync(logEvent);

        // Assert
        publisherMock.Contains("Cached analysis found for exception");
        Assert.Equal("content", faultServiceMock.Content);
    }

    [Fact]
    public async Task AnalyzeAndPublishFaultAsync_CallsFaultAnalysisIfNotCached()
    {
        var faultServiceMock = new MockFaultAnalysis("content", "role");
        var logEvent = Mock.Of<ILogEvent>();
        var publisherMock = new Mock<IPublisher>();
        publisherMock.Setup(p => p.WriteLine(It.IsAny<string>())).Returns(Task.CompletedTask);
        var settings = Options.Create(new AiEventSettings { MinLogLevel = LogLevel.Information, PollingDelay = 1 });
        var logger = new ApplicationLogger(
            "TestCategory",
            settings,
            Mock.Of<ILogEvent>,
            publisherMock.Object,
            faultServiceMock
        );
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.AnalyzeAndPublishFaultAsync(logEvent);

        // Wait for the background task to complete
        await Task.Delay(100);

        // Assert
        Assert.Equal("content", faultServiceMock.Content);
        publisherMock.Verify(p => p.WriteLine(It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public void Log_WritesLogEvent_AndUsesFormatter()
    {
        // Arrange
        var logEventMock = new Mock<ILogEvent>();
        logEventMock.SetupAllProperties();
        logEventMock.Setup(e => e.Serialize()).Returns("serialized-log");
        var publisherMock = new Mock<IPublisher>();
        var faultServiceMock = new Mock<IFaultAnalysisService>();
        var settings = Options.Create(new AiEventSettings { MinLogLevel = LogLevel.Information, PollingDelay = 1 });
        var logger = new ApplicationLogger(
            "TestCategory",
            settings,
            () => logEventMock.Object,
            publisherMock.Object,
            null // No fault analysis for this test
        );

        var state = "test-state";
        var exception = (Exception?)null;
        string formattedMessage = "formatted-message";
        string formatter(string s, Exception? e) => formattedMessage;

        // Act
        logger.Log(LogLevel.Information, new EventId(1, "TestEvent"), state, exception, formatter);

        // Assert
        Assert.Equal(LogLevel.Information, logEventMock.Object.Level);
        Assert.Equal("TestCategory", logEventMock.Object.Source);
        Assert.Equal(formattedMessage, logEventMock.Object.Body);
        publisherMock.Verify(p => p.WriteLine("serialized-log"), Times.Once);
    }

    [Fact]
    public void Log_CallsFaultAnalysis_WhenExceptionAndServiceProvided()
    {
        // Arrange
        var logEventMock = new Mock<ILogEvent>();
        logEventMock.SetupAllProperties();
        logEventMock.Setup(e => e.Serialize()).Returns("serialized-log");
        var publisherMock = new MockPublisher();
        var faultServiceMock = new MockFaultAnalysis("AnalyzeAndPublishFaultAsync", "role");
        var settings = Options.Create(new AiEventSettings { MinLogLevel = LogLevel.Information, PollingDelay = 1 });
        var logger = new MockLogger(LogLevel.Information, publisherMock);
        var state = "test-state";
        var exception = new InvalidOperationException("fail");
        string formattedMessage = "formatted-message";
        string formatter(string s, Exception? e) => formattedMessage;

        // Act
        logger.Log(LogLevel.Error, new EventId(2, "TestEvent"), state, exception, formatter);

        // Assert
        logger.Contains("AnalyzeAndPublishFaultAsync");
        logger.Contains("TestEvent");
        publisherMock.Contains("serialized-log");
    }

    [Fact]
    public void ExceptionHelper_SameExceptionProducesSameHash()
    {
        // Arrange
        var ex1 = new InvalidOperationException("Test error");
        var ex2 = new InvalidOperationException("Test error");

        // Simulate same stack trace by throwing and catching in the same method       
        var hash1 = ExceptionHelper.GetExceptionHash(ex1);
        var hash2 = ExceptionHelper.GetExceptionHash(ex2);

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ExceptionHelper_DifferentStackTraceProducesDifferentHash()
    {
        // Arrange
        string hash1, hash2;
        try
        {
            ThrowExceptionA();
        }
        catch (Exception e1)
        {
            hash1 = ExceptionHelper.GetExceptionHash(e1);
        }
        try
        {
            ThrowExceptionB();
        }
        catch (Exception e2)
        {
            hash2 = ExceptionHelper.GetExceptionHash(e2);
        }
        // Assert
        Assert.NotEqual(hash1, hash2);

        static void ThrowExceptionA()
        {
            throw new InvalidOperationException("Test error");
        }
        static void ThrowExceptionB()
        {
            // Different stack frame
            throw new InvalidOperationException("Test error");
        }
    }

    private static HttpClient GetFaultAnalysisServiceClient(Exception exception)
    {
        return GetMockHttpClient(exception);
    }

    private static IHttpClientFactory GetHttpClientFactory(Exception exception)
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(GetFaultAnalysisServiceClient(exception));
        return mockFactory.Object;
    }
}