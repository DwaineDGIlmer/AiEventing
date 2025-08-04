# Application Logging

This folder contains classes for application-specific logging providers, factories, and logger implementations. These components integrate with the .NET logging infrastructure and provide advanced features such as custom log event factories, external publishers, and optional AI-driven fault analysis.

---

## Classes

### `ApplicationLogProvider`
Implements `ILoggerProvider` to create loggers for application-specific logging.

- Configurable via `AiEventSettings`.
- Supports custom log event factories for flexible log event creation.
- Allows optional external publishers for log output (e.g., console, file, remote).
- Can integrate with a fault analysis service for diagnosing system faults.
- Typically used as a provider within the .NET logging infrastructure.

**Usage Example:**
```csharp
var provider = new ApplicationLogProvider(
    settings,
    () => new MyLogEvent(),
    publisher: new ConsolePublisher(),
    faultAnalysis: myFaultAnalysisService
);

var logger = provider.CreateLogger("MyCategory");
logger.LogInformation("Application started.");
```

---

### `ApplicationLogFactory`
Implements `ILoggerFactory` to create and manage loggers and logger providers for the application.

- Supports registration of custom `ILoggerProvider` implementations.
- Integrates with external publishers and optional fault analysis services.
- Manages logger instances by category and ensures consistent configuration.

**Usage Example:**
```csharp
var factory = new ApplicationLogFactory(
    settings,
    () => new MyLogEvent(),
    publisher: new ConsolePublisher(),
    faultAnalysisService: myFaultAnalysisService
);

factory.AddProvider(new ApplicationLogProvider(settings, () => new MyLogEvent()));
var logger = factory.CreateLogger("MyCategory");
logger.LogWarning("This is a warning message.");
```

---

### `ApplicationLogger`
Implements `ILogger` to provide structured and scoped logging with support for customizable event creation, external scope providers, and fault analysis services.

- Supports structured logging with custom log event objects.
- Integrates with external publishers for log output.
- Supports logical operation scoping and minimum log level filtering.
- Optionally analyzes exceptions using a fault analysis service and publishes results.

**Usage Example:**
```csharp
var logger = new ApplicationLogger(
    "MyCategory",
    settings,
    () => new MyLogEvent(),
    publisher: new ConsolePublisher(),
    faultAnalysisService: myFaultAnalysisService
);

using (logger.BeginScope("OperationScope"))
{
    logger.LogError(new Exception("Something went wrong!"), "An error occurred.");
}
```

---

## Related Documentation

- For core configuration and settings, see [Core/Configuration/README.md](../../Core/Configuration/README.md)
- For log event contracts and interfaces, see [Core/Contracts/README.md](../../Core/Contracts/README.md)
- For fault analysis services, see [Core/Services/README.md](../../Core/Services/README.md)

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## License

This project is licensed under the MIT License.

---

## Contacts

For questions or support, please contact