using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Loggers.Contracts;

/// <summary>
/// Represents a log event containing details such as timestamp, message, log level, source, exception, and tracing information.
/// </summary>
/// <remarks>
/// This class encapsulates all relevant information for a single log entry, including optional exception and tracing data.
/// </remarks>
public interface ILogEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred, including the offset from Coordinated Universal Time (UTC).
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    /// </summary>
    DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets the log message describing the event.
    /// </summary>
    string Body { get; set; }

    /// <summary>
    /// Gets the trace identifier for distributed tracing.
    /// </summary>
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    string TraceId { get; set; }

    /// <summary>
    /// Gets the span identifier for distributed tracing.
    /// </summary>
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    string SpanId { get; set; }

    /// <summary>
    /// Gets the severity level of the log event.
    /// </summary>
    /// <remarks>This is the log leve as defined by Microsoft, https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel</remarks>
    LogLevel Level { get; set; }

    /// <summary>
    /// Gets the source or origin of the log event.
    /// </summary>
    /// <remarks>This is the name of the class or component that generated the log event.</remarks>
    string Source { get; set; }

    /// <summary>
    /// Gets the correlation identifier for distributed tracing.
    /// </summary>
    /// <remarks>Used for correlating this event from an application event perspective.</remarks>
    string? CorrelationId { get; set; }

    /// <summary>
    /// Gets the exception associated with the log event, if any.
    /// </summary>
    /// <value>
    /// An <see cref="Exception"/> instance if an exception occurred; otherwise, <c>null</c>.
    /// </value>
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    Exception? Exception { get; set; }

    /// <summary>
    /// Gets the stack trace associated with the log event, if available.
    /// </summary>
    /// <value>
    /// An optional <see cref="StackTrace"/> object that provides information about the call stack at the time the event was logged, or <c>null</c> if not applicable.
    /// </value>
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    StackTrace? StackTrace { get; set; }

    /// <summary>
    /// Serializes the current object to its string representation.
    /// </summary>
    /// <returns>A string containing the serialized representation of the object.</returns>
    string Serialize();
}
