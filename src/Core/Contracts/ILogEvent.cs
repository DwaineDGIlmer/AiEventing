using Core.Models;
using Microsoft.Extensions.Logging;

namespace Core.Contracts;

/// <summary>
/// Represents a log event containing details such as timestamp, message, log level, source, exception, and tracing information.
/// </summary>
/// <remarks>
/// This class encapsulates all relevant information for a single log entry, including optional exception and tracing data.
/// </remarks>
public interface ILogEvent
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    /// <remarks>This should be an id the can be used to uniquely identify this fault not this instance.</remarks> 
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the application.
    /// </summary>
    public string ApplicationId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the component.
    /// </summary>
    public string ComponentId { get; set; }

    /// <summary>
    /// Gets or sets the name of the environment in which the application is running.
    /// </summary>
    public string Environment { get; set; }

    /// <summary>
    /// Gets or sets the version of the application or component.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the deployment.
    /// </summary>
    public string DeploymentId { get; set; }

    /// <summary>
    /// Gets or sets a collection of key-value pairs representing metadata tags.
    /// </summary>
    /// <remarks>Use this property to store and retrieve metadata associated with an object.  Keys should be
    /// unique within the dictionary to avoid overwriting values.</remarks>
    public IDictionary<string, string> Tags { get; set; }

    /// <summary>
    /// Gets the date and time when the event occurred, including the offset from Coordinated Universal Time (UTC).
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets the log message describing the event.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets the trace identifier for distributed tracing.
    /// </summary>
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    public string TraceId { get; set; }

    /// <summary>
    /// Gets the span identifier for distributed tracing.
    /// </summary>
    /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
    public string SpanId { get; set; }

    /// <summary>
    /// Gets the severity level of the log event.
    /// </summary>
    /// <remarks>This is the log leve as defined by Microsoft, https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel</remarks>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets the source or origin of the log event.
    /// </summary>
    /// <remarks>This is the name of the class or component that generated the log event.</remarks>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the line number of  the code where the log event was generated.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets the correlation identifier for distributed tracing.
    /// </summary>
    /// <remarks>Used for correlating this event from an application event perspective.</remarks>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the type of the exception represented as a string.
    /// </summary>
    public SerializableException? Exception { get; set; }

    /// <summary>
    /// Gets or sets the collection of inner exceptions associated with the current operation.
    /// </summary>
    public IList<SerializableException> InnerExceptions { get; set; }

    /// <summary>
    /// Serializes the current object to its string representation.
    /// </summary>
    /// <returns>A string containing the serialized representation of the object.</returns>
    public string Serialize();
}