# 🧠 Loggers.Models

This folder contains data models used by the logging infrastructure, with a focus on structured and OpenTelemetry-compliant log events.

---

## 📦 Classes

### `OtelLogEvents`
Represents an OpenTelemetry (OTEL) log event with structured fields for tracing, severity, and exception details. Implements the `ILogEvent` interface.

**Key Features:**
- Captures standard log event fields such as timestamp, severity, message body, trace and span IDs, source, environment, version, deployment ID, and tags.
- Supports structured exception details via the `SerializableException` model and a collection of inner exceptions.
- Provides a unique identifier for each log event, based on the exception or message body.
- Serializes log events to a JSON format that is compatible with OpenTelemetry, omitting null values and including relevant attributes.

**Key Properties:**
- `string Id`: Unique identifier for the log event (hash of exception or body).
- `string ApplicationId`, `string ComponentId`, `string Environment`, `string Version`, `string DeploymentId`
- `IDictionary<string, string> Tags`: Metadata tags.
- `DateTimeOffset Timestamp`: UTC timestamp of the log event.
- `string Body`: Main log message.
- `string TraceId`, `string SpanId`: Distributed tracing identifiers.
- `LogLevel Level`: Severity of the event.
- `string Source`: Source of the log event.
- `int LineNumber`: Line number in the source code.
- `string? CorrelationId`: Optional correlation ID for distributed tracing.
- `SerializableException? Exception`: Exception details, if any.
- `IList<SerializableException> InnerExceptions`: Inner exceptions, if any.

**Key Method:**
- `string Serialize()`: Serializes the log event to a JSON string in OpenTelemetry-compliant format.

**Usage Example:**
```csharp
var logEvent = new OtelLogEvents
{
    ApplicationId = "MyApp",
    ComponentId = "Worker",
    Environment = "Production",
    Version = "1.0.0",
    DeploymentId = "deploy-123",
    Body = "An error occurred during processing.",
    Level = LogLevel.Error,
    TraceId = "abc123",
    SpanId = "def456",
    Source = "MyApp.Worker",
    Exception = new SerializableException(new Exception("Something went wrong"))
};

string otelJson = logEvent.Serialize();
// Send otelJson to your log pipeline or OpenTelemetry collector
```

---

## 🔗 Related Documentation

- For log event contracts and interfaces, see [../../Core/Contracts/README.md](../../Core/Contracts/README.md)
- For exception serialization, see [../../Core/Models/README.md](../../Core/Models/README.md)

---

## 🤝 Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.  
See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

---

## 📄 License

This project is licensed under the [MIT License](../../LICENSE).

---

## 📬 Contact

For questions or support, please contact Dwaine Gilmer at [Protonmail.com](mailto:dwaine.gilmer@protonmail.com) or submit an issue on the project's GitHub
