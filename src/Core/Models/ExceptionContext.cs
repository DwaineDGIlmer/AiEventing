using Core.Serializers;
using Domain.Fault;
using System.Diagnostics;

namespace Core.Models
{
    /// <summary>
    /// Represents detailed contextual information about an exception, including its type, message, stack trace,  and
    /// additional metadata for tracking and diagnostics.
    /// </summary>
    /// <remarks>This class provides a comprehensive set of properties to capture and analyze exception
    /// details,  including the exception type, message, stack trace, source, and method where the exception occurred. 
    /// It also includes metadata such as correlation IDs, severity levels, and custom properties for  enhanced
    /// diagnostics and troubleshooting.  Use this class to encapsulate exception data for logging, monitoring, or
    /// reporting purposes.  The properties are designed to support scenarios such as distributed tracing, error
    /// tracking,  and debugging in complex systems.</remarks>
    sealed public class ExceptionContext : Exceptions
    {
        /// <summary>Unique identifier for the incident.</summary>
        public string Id { get; set; } = string.Empty;

        ///<summary> Gets or sets the name of the application.</summary>
        public string Application { get; set; } = string.Empty;

        /// <summary>Gets or sets the timestamp representing the current date and time in ISO 8601 format.</summary>
        /// <remarks>The timestamp is initialized to the current UTC date and time when the property is first set.</remarks>
        public DateTimeOffset Timestamp { get; set; }

        /// <summary> Gets or sets the severity level of the current operation or event. </summary>
        public LogLevel SeverityLevel { get; set; } = LogLevel.Error;

        /// <summary>Gets or sets the unique identifier for the fault.</summary>
        public string FaultId { get; set; } = string.Empty;

        /// <summary>Gets or sets the trace identifier associated with the log event.</summary>
        public string TraceId { get; set; } = Activity.Current is not null ? Activity.Current.TraceId.ToString() : string.Empty;

        /// <summary>Gets or sets the span identifier associated with the log event.</summary>
        public string SpanId { get; set; } = Activity.Current is not null ? Activity.Current.SpanId.ToString() : string.Empty;

        /// <summary>Gets or sets the exception associated with the log event, if any.</summary>
        public SerializableException? Exception { get; set; } = null;

        /// <summary>Gets or sets the type of the exception as a string representation.</summary>
        new public string ExceptionType
        {
            get => _exceptionType;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var trimmed = value.Trim();
                    _exceptionType = trimmed.Contains('.')
                        ? trimmed[(trimmed.LastIndexOf('.') + 1)..]
                        : trimmed;
                }
                else
                {
                    _exceptionType = string.Empty;
                }
            }
        }
        private string _exceptionType = string.Empty;

        /// <summary>Gets or sets the stack trace information associated with an error or exception.</summary>
        public string StackTrace { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the service associated with the current operation.</summary>
        public string Service { get; set; } = string.Empty;

        /// <summary>The Environment that this fault occurred in, e.g., Production, Staging, Development.</summary>
        public string Environment { get; set; } = string.Empty;

        /// <summary>Gets or sets the path to the source file.</summary>
        public string SourceFile { get; set; } = string.Empty;

        /// <summary>Gets or sets the line number associated with the current operation or context.</summary>
        public int LineNumber { get; set; }

        /// <summary>Gets or sets the message associated with the exception.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Gets or sets the source identifier associated with the current object.</summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>Gets or sets the HTTP method to be used for the request.</summary>
        public string Method { get; set; } = string.Empty;

        /// <summary>Gets or sets the name of the target site.</summary>
        public string TargetSite { get; set; } = string.Empty;

        /// <summary>Gets or sets the exception instance that caused the current exception. </summary>
        public IList<SerializableException> InnerException { get; set; } = [];

        /// <summary>Gets or sets the identifier of the currently managed thread.</summary>
        public int ThreadId { get; set; } = System.Environment.CurrentManagedThreadId;

        /// <summary>Gets or sets the unique identifier for the exception. </summary>
        public string ExceptionId { get; set; } = Guid.NewGuid().ToString();

        /// <summary> Gets or sets the unique identifier for the occurrence of an exception. </summary>
        public string ExceptionOccurrenceId { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Gets or sets the unique identifier used to correlate related operations or requests. </summary>
        public string? CorrelationId { get; set; }

        /// <summary> Gets or sets the schema data for the hotel, represented as a dictionary of key-value pairs.</summary>
        public string Otelschema
        {
            get
            {
                var otelLog = new Dictionary<string, object?>
                {
                    ["timestamp"] = Timestamp.ToUnixTimeMilliseconds() * 1_000_000, // nanoseconds
                    ["severity_text"] = SeverityLevel.ToString().ToUpperInvariant(),
                    ["severity_number"] = (int)SeverityLevel,
                    ["body"] = Message,
                    ["trace_id"] = string.IsNullOrWhiteSpace(TraceId) ? null : TraceId,
                    ["span_id"] = string.IsNullOrWhiteSpace(SpanId) ? null : SpanId,
                };

                // Remove nulls for OTEL compliance
                var attributes = new Dictionary<string, object?>();
                if (Source is not null)
                    attributes["source"] = Source;
                if (CorrelationId is not null)
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

        /// <summary>
        /// Gets or sets a collection of custom properties associated with the object.
        /// </summary>
        /// <remarks>This property allows dynamic storage of additional information that may not be
        /// explicitly defined in the object's structure. Keys should be unique within the dictionary to avoid
        /// overwriting values.</remarks>
        public IDictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();
    }
}
