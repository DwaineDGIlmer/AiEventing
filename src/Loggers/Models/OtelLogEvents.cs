using Core.Extensions;
using Core.Serializers;
using Loggers.Contracts;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("LoggerBenchMarkTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
[assembly: InternalsVisibleTo("IntegrationTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Loggers.Models;

/// <summary>
/// Represents an OpenTelemetry (OTEL) log event with structured fields for tracing, severity, and exception details.
/// </summary>
internal class OtelLogEvents : ILogEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OtelLogEvents"/> class with default values.
    /// </summary>
    public OtelLogEvents()
    {
        Timestamp = DateTimeOffset.UtcNow;
        Body = string.Empty;
        TraceId = string.Empty;
        SpanId = string.Empty;
        Level = LogLevel.Information;
        Source = string.Empty;
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
    /// Gets or sets the optional correlation identifier for distributed tracing.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Gets or sets the exception associated with the log event, if any.
    /// </summary>
    public Exception? Exception { get; set; }

    /// <summary>
    /// Gets or sets the stack trace associated with the log event, if any.
    /// </summary>
    public StackTrace? StackTrace { get; set; }

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
            ["attributes"] = new Dictionary<string, object?>
            {
                ["source"] = Source,
                ["correlation_id"] = CorrelationId,
                ["exception.type"] = Exception?.GetType().FullName,
                ["exception.message"] = Exception?.Message,
                ["exception.stacktrace"] = Exception != null ? (StackTrace?.ToString() ?? Exception.StackTrace) : null
            }
        };

        // Remove nulls for OTEL compliance
        otelLog["attributes"] = ((Dictionary<string, object?>)otelLog["attributes"]!).RemoveNullValues();

        // Will fail if the instance is not initialized
        return JsonConvertService.Instance!.Serialize(otelLog);
    }
}