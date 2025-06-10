using Core.Configuration;
using Core.Contracts;
using Core.Serializers;
using Loggers.Application;
using Loggers.Models;
using Loggers.Publishers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace IntegrationTests.Loggers;
/// <summary>
/// This test sets up a real DI container, registers the logger, and uses ConsolePublisher to verify that a log event is processed.
/// The test checks that the publisher's TotalEvents counter increases, indicating the log was processed.
/// </summary>
public class BaseLoggerIntegration
{
    public BaseLoggerIntegration()
    {
        if (!JsonConvertService.IsInitialized)
        {
            var settings = new AiEventSettings();
            JsonConvertService.Initialize(new JsonSerializerOptions()
            {
                WriteIndented = settings.WriteIndented,
                DefaultIgnoreCondition = settings.DefaultIgnoreCondition,
                Encoder = settings.UnsafeRelaxedJsonEscaping ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping : null
            });
        }
    }

    [Fact]
    public async Task Logger_Writes_LogEvent_To_ConsolePublisher()
    {
        // Arrange: Setup DI and logger
        var services = new ServiceCollection();

        // Minimal AiEventSettings for test
        var settings = new AiEventSettings { MinLogLevel = LogLevel.Information, PollingDelay = 1 };

        // Use a test publisher to capture output
        var testPublisher = new ConsolePublisher(settings.PollingDelay);

        // Use a simple log event factory
        static ILogEvent logEventFactory() => new OtelLogEvents();

        // Register ApplicationLogProvider with test publisher
        services.AddSingleton<ILoggerProvider>(sp =>
            new ApplicationLogProvider(settings, logEventFactory, testPublisher));

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(new ApplicationLogProvider(settings, logEventFactory, testPublisher));
        });

        var provider = services.BuildServiceProvider();
        var logger = provider.GetRequiredService<ILogger<BaseLoggerIntegration>>();

        // Act: Log a message
        logger.LogInformation("Integration test log message");

        // Allow background worker to process the queue
        await Task.Delay(2000);

        // Assert: The publisher should have processed at least one event
        Assert.True(testPublisher.TotalEvents > 0);

        await testPublisher.DisposeAsync();
    }
}