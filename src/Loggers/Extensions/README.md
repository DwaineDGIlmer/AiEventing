# Loggers.Extensions

This folder contains extension methods for integrating custom logging providers into the application's dependency injection and logging infrastructure.

---

## Classes

### `ServiceCollectionExtensions`
Provides extension methods for `IServiceCollection` and `ILoggingBuilder` to configure and initialize logging services, including registration of the custom `ApplicationLogProvider` and binding of `AiEventSettings`.

**Key Methods:**
- `InitializeLogging(this IServiceCollection services, IConfiguration configuration)`:  
  Registers the `ApplicationLogProvider` with the logging system, applies logging configuration from the provided `IConfiguration`, and binds `AiEventSettings` from configuration.

- `AddFaultAnalysisLogger(this ILoggingBuilder builder, IConfiguration configuration)`:  
  Registers the `ApplicationLogProvider` as a singleton logger provider, binds settings from configuration, and resolves optional dependencies such as `IFaultAnalysisService` and `IPublisher`. Also applies Microsoft logging configuration from the "Logging" section of the configuration.

**Usage Example:**
```csharp
// In your application's startup or Program.cs:
services.InitializeLogging(configuration);

// Or, for more granular control:
services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddFaultAnalysisLogger(configuration);
});
```

These methods ensure that your application's logging is configured to use the custom provider, with support for advanced features such as AI-driven fault analysis and external log publishing.

---

## Related Documentation

- For the implementation of the custom logger, see [../Application/README.md](../Application/README.md)
- For core configuration and settings, see [../../Core/Configuration/README.md](../../Core/Configuration/README.md)
- For log event contracts and interfaces, see [../../Core/Contracts/README.md](../../Core/Contracts/README.md)

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## License

This project is licensed under the MIT License.

---

## Contacts

For questions or support, please contact the project