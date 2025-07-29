using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using Core.Helpers;
using Core.Models;
using Loggers.Contracts;
using Loggers.Publishers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Loggers.Application;

/// <summary>
/// Provides a structured and scoped logging implementation with support for customizable event creation, external scope
/// providers, and fault analysis services.
/// </summary>
/// <remarks>The <see cref="ApplicationLogger"/> class is designed for applications requiring advanced logging
/// capabilities, including support for different log levels, logical operation scoping, and integration with external
/// scope providers. It allows for customizable log event creation and publishing, and optionally integrates with a
/// fault analysis service for enhanced debugging and diagnostics.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationLogger"/> class.
/// This logger supports structured, scoped logging with customizable event creation and publishing.
/// It allows integration with external scope providers, fault analysis services, and supports configurable minimum log levels.
/// If no publisher or scope provider is specified, defaults are used for console output and scope management.
/// </remarks>
/// <param name="categoryName">The category name for messages produced by this logger.</param>
/// <param name="settings">The <see cref="AiEventSettings"/> used to configure the logger.</param>
/// <param name="factory">A factory function to create <see cref="ILogEvent"/> instances.</param>
/// <param name="publisher">Optional: Publishes log events. Defaults to <see cref="ConsolePublisher"/> if not provided.</param>
/// <param name="faultAnalysisService">Optional: Service for analyzing faults in the system.</param>
/// 
/// <exception cref="ArgumentNullException">
/// Thrown if <paramref name="factory"/> is <c>null</c>.
/// </exception>
public class ApplicationLogger(
    string categoryName,
    IOptions<AiEventSettings> settings,
    Func<ILogEvent> factory,
    IPublisher? publisher = null,
    IFaultAnalysisService? faultAnalysisService = null) : ILogger
{
    #region Internal Fields    
    /// <summary>
    /// Gets the collection of log events, indexed by their unique identifiers.
    /// </summary>
    /// <remarks>This property provides access to the internal log events for tracking and managing logging
    /// operations. The dictionary is thread-safe, allowing concurrent access in multi-threaded environments.</remarks>
    internal ConcurrentDictionary<string, ILogEvent> LogEventsCache { get; } = new();

    /// <summary>
    /// Gets a <see cref="Publisher"/> instance used to direct tracing or debugging output to the console.
    /// </summary>
    internal IPublisher Publisher { get; set; } = publisher ?? new ConsolePublisher(settings.Value.PollingDelay);

    /// <summary>
    /// Gets or sets the service responsible for analyzing faults in the system.
    /// </summary>
    internal IFaultAnalysisService? FaultAnalysisService { get; set; } = faultAnalysisService;

    /// <summary>
    /// Gets the provider that supplies external scope information for logging.
    /// </summary>
    internal IExternalScopeProvider ScopeProvider { get; } = new LoggerExternalScopeProvider();

    /// <summary>
    /// Gets the factory method used to create log events.
    /// </summary>  
    internal Func<ILogEvent> LogEventFactory { get; } = factory.IsNullThrow("LogEventFactory cannot be null.");

    /// <summary>
    /// The settings used for configuring the logger. 
    /// </summary>
    internal AiEventSettings Settings { get; } = settings.Value.IsNullThrow();
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets the name of the category for this logger.
    /// </summary>
    public string CategoryName { get; } = categoryName.IsNullThrow();

    /// <summary>
    /// Gets the minimum <see cref="LogLevel"/> that will be logged by the logger.
    /// Messages with a lower log level will be ignored.
    /// </summary>
    /// <remarks>
    /// The <see cref="ApplicationLogger"/> class allows logging messages with different log levels, supports scoping for logical operations,
    /// and integrates with external scope providers for enhanced context management. It is designed to be used in applications that require
    /// structured and scoped logging.
    /// </remarks>
    public LogLevel MinLogLevel { get; } = settings.Value.MinLogLevel;

    #endregion

    #region Public Methods
    /// <summary>
    /// Begins a logical operation scope for logging.
    /// </summary>
    /// <typeparam name="TState">The type of the state to associate with the scope. Must be non-null.</typeparam>
    /// <param name="state">The identifier for the scope. This is typically used to group a set of log entries.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that ends the logical operation scope on disposal, or <c>null</c> if the scope could not be created.
    /// </returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        // Use the scope provider to push the new scope
        return ScopeProvider.Push(state);
    }

    /// <summary>
    /// Determines whether logging is enabled for the specified <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>
    /// <c>true</c> if the specified <paramref name="logLevel"/> is greater than or equal to the configured minimum log level; otherwise, <c>false</c>.
    /// </returns>
    public bool IsEnabled(LogLevel logLevel)
    {
        // Only enable log levels greater than or equal to the configured LogLevel
        return logLevel >= MinLogLevel && Settings.LoggingEnabled;
    }

    /// <summary>
    /// Logs a message with the specified log level, event ID, state, exception, and formatter.
    /// </summary>
    /// <typeparam name="TState">The type of the state object to be logged.</typeparam>
    /// <param name="logLevel">The severity level of the log entry.</param>
    /// <param name="eventId">The identifier for the log event.</param>
    /// <param name="state">The state to be logged.</param>
    /// <param name="exception">The exception related to this log entry, if any.</param>
    /// <param name="formatter">
    /// A function that creates a string message from the state and exception.
    /// </param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        // Create a new log event using the factory method
        var logEvent = LogEventFactory();

        // Populate other logEvent properties as needed
        logEvent.Timestamp = DateTimeOffset.UtcNow;
        logEvent.ApplicationId = Settings.ApplicationId ?? string.Empty;
        logEvent.ComponentId = Settings.ComponentId ?? string.Empty;
        logEvent.DeploymentId = Settings.DeploymentId ?? string.Empty;
        logEvent.Environment = Settings.Environment ?? string.Empty;
        logEvent.Level = logLevel;
        logEvent.Source = CategoryName;
        logEvent.Exception = exception is not null ? new SerializableException(exception) : null;
        logEvent.CorrelationId = Activity.Current?.RootId ?? Guid.NewGuid().ToString();
        logEvent.TraceId = Activity.Current?.TraceId.ToString() ?? string.Empty;
        logEvent.SpanId = Activity.Current?.SpanId.ToString() ?? string.Empty;
        logEvent.InnerExceptions = ExceptionHelper.GetInnerExceptions(exception);

        // Handle scopes: collect all scopes into a string and prepend to Body
        var scopeBuilder = new System.Text.StringBuilder();
        ScopeProvider.ForEachScope((scope, sb) =>
        {
            if (sb.Length > 0) sb.Append(" | ");
            sb.Append(scope?.ToString());
        }, scopeBuilder);

        var message = formatter(state, exception);

        if (scopeBuilder.Length > 0)
        {
            logEvent.Body = $"[Scopes: {scopeBuilder}] {message}";
        }
        else
        {
            logEvent.Body = message;
        }

        logEvent.Id = exception is not null ?
            ExceptionHelper.GetExceptionHash(exception) :
            ExceptionHelper.GetExceptionHash(logEvent.Body);

        if (exception != null && FaultAnalysisService != null && Settings.RcaServiceEnabled)
        {
            AnalyzeAndPublishFaultAsync(logEvent);
        }

        Publisher.WriteLine(logEvent.Serialize());
    }

    /// <summary>
    /// Analyzes the provided exception to identify potential causes or fixes and publishes the resulting fault
    /// analysis.
    /// </summary>
    /// <remarks>If the exception has been previously analyzed and cached, the cached fault analysis is
    /// published immediately. Otherwise, the method performs an asynchronous fault analysis using the <see
    /// cref="FaultAnalysisService"/>  and publishes the results. If an error occurs during the analysis, an error log
    /// event is published instead.</remarks>
    /// <param name="logEvent">The log event associated with the exception. This parameter cannot be <see langword="null"/>.</param>
    /// 
    internal void AnalyzeAndPublishFaultAsync(ILogEvent logEvent)
    {
        Task.Run(async () =>
        {
            string? exMessage = null;
            try
            {
                var fault = logEvent;
                if (LogEventsCache.TryGetValue(logEvent.Id, out var cachedEvent) && cachedEvent.IsNotNull())
                {
                    // Only logging if debug is enabled
                    if (IsEnabled(LogLevel.Debug))
                    {
                        await Publisher.WriteLine($"Cached analysis found for exception: {logEvent.Body}");
                    }
                    fault = cachedEvent;
                }
                else
                {
                    _ = LogEventsCache.TryAdd(logEvent.Id, fault);
                }

                if (!await FaultAnalysisService!.AnalyzeFaultAsync(fault))
                {
                    exMessage = $"Fault analysis failed for: {logEvent.Body}";
                }
            }
            catch (Exception ex)
            {
                exMessage = $"Exception during fault analysis: {ex}";
            }

            if (!string.IsNullOrEmpty(exMessage))
            {
                var errorEvent = LogEventFactory();
                errorEvent.Timestamp = DateTimeOffset.UtcNow;
                errorEvent.Level = LogLevel.Error;
                errorEvent.Source = CategoryName;
                errorEvent.Body = exMessage;

                await Publisher.WriteLine(errorEvent.Serialize());
            }
        });
    }
    #endregion
}