# Loggers

This project provides a flexible, extensible logging infrastructure for .NET applications, supporting OpenTelemetry-compliant log events, asynchronous publishing, and integration with Microsoft.Extensions.Logging.

## Features

- **OpenTelemetry-compliant log event serialization**
- **Customizable log event model via `ILogEvent`**
- **Asynchronous, thread-safe publishing with `ConsolePublisher`**
- **Integration with Microsoft.Extensions.Logging abstractions**
- **Pluggable publisher model via `IPublisher`**
- **Configurable via dependency injection and settings**

## Key Components

- **ILogEvent**  
  Interface representing a log event, including timestamp, message, severity, tracing, and exception details.

- **IPublisher**  
  Interface for publishing log messages asynchronously. Implementations can target console, files, remote endpoints, etc.

- **ConsolePublisher**  
  Default publisher that writes log messages to the console using background processing for high throughput.

- **ApplicationLogProvider / ApplicationLogFactory**  
  Integrate with Microsoft.Extensions.Logging to provide structured, OpenTelemetry-ready logging for your application.

## Usage

### Registering the Logger

```csharp
// In your DI setup (e.g., Startup.cs or Program.cs)
services.InitializeServices(builder.Configuration);
services.InitializeLogging(builder.Configuration);
```