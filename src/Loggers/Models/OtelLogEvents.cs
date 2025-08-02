using Core.Contracts;
using Core.Extensions;
using Core.Helpers;
using Core.Models;
using Core.Serializers;
using Microsoft.Extensions.Logging;

namespace Loggers.Models;

/// <summary>
/// Represents an OpenTelemetry (OTEL) log event with structured fields for tracing, severity, and exception details.
/// </summary>
public class OtelLogEvents : ILogEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OtelLogEvents"/> class with default values.
    /// </summary>
    public OtelLogEvents()
    {
        Timestamp = DateTimeOffset.UtcNow;

        ApplicationId = string.Empty;
        ComponentId = string.Empty;

        Level = LogLevel.Error;
        Body = string.Empty;
        TraceId = string.Empty;
        SpanId = string.Empty;
        Source = string.Empty;
        Environment = string.Empty;
        Version = string.Empty;
        DeploymentId = string.Empty;
        LineNumber = 0;
    }

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public string Id
    {
        get => Exception is not null ?
            ExceptionHelper.GetExceptionHash(Exception) :
            ExceptionHelper.GetExceptionHash(Body);
        set { /* This property is read-only; no setter implementation needed */ }
    }

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
    public IDictionary<string, string> Tags { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the fully qualified name of the type of the exception.
    /// </summary>
    public string ExceptionType
    {
        get => Exception?.GetType().FullName ?? string.Empty;
        set { /* This property is read-only; no setter implementation needed */ }
    }

    /// <summary>
    /// Gets or sets the timestamp of the log event in UTC.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the main log message body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier associated with the log event.
    /// </summary>
    public string TraceId { get; set; }

    /// <summary>
    /// Gets or sets the span identifier associated with the log event.
    /// </summary>
    public string SpanId { get; set; }

    /// <summary>
    /// Gets or sets the log level (severity) of the event.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the source of the log event.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the line number associated with the current operation or context.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Gets or sets the optional correlation identifier for distributed tracing.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the exception associated with the log event, if any.
    /// </summary>
    public SerializableException? Exception { get; set; } = null;

    /// <summary>
    /// Gets or sets the collection of inner exceptions associated with the current exception.
    /// </summary>
    public IList<SerializableException> InnerExceptions
    {
        get
        {
            if (Exception is null)
                return [];
            return Exception.InnerExceptions;
        }
        set
        {
            // If the value is null, we do not set it to avoid null reference exceptions.
        }
    }

    /// <summary>
    /// Serializes the log event to a JSON string in OpenTelemetry-compliant format, omitting null values.
    /// </summary>
    /// <returns>A JSON string representing the log event.</returns>
    public string Serialize()
    {
        var otelLog = new Dictionary<string, object?>
        {
            ["timestamp"] = Timestamp.ToUnixTimeMilliseconds() * 1_000_000, // nanoseconds
            ["severity_text"] = Level.ToString().ToUpperInvariant(),
            ["severity_number"] = (int)Level,
            ["body"] = Body,
            ["trace_id"] = string.IsNullOrWhiteSpace(TraceId) ? null : TraceId,
            ["span_id"] = string.IsNullOrWhiteSpace(SpanId) ? null : SpanId,
        };

        // Remove nulls for OTEL compliance
        var attributes = new Dictionary<string, object?>();
        if (!Source.IsNullOrEmpty())
            attributes["source"] = Source;
        if (!CorrelationId.IsNullOrEmpty())
            attributes["correlation_id"] = CorrelationId;
        if (!string.IsNullOrWhiteSpace(Exception?.ExceptionType))
            attributes["exception.type"] = Exception.ExceptionType;
        if (!string.IsNullOrWhiteSpace(Exception?.ExceptionMessage))
            attributes["exception.message"] = Exception.ExceptionMessage;
        if (!string.IsNullOrWhiteSpace(Exception?.ExceptionStackTrace))
            attributes["exception.stacktrace"] = Exception.ExceptionStackTrace;
        otelLog["attributes"] = attributes;

        // Will fail if the instance is not initialized
        return JsonConvertService.Instance!.Serialize(otelLog);
    }
}