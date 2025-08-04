# Publishers

This folder contains implementations of the `IPublisher` interface, which enable flexible and extensible log event publishing for your application. Publishers can be injected into the custom `ILogger` implementation, allowing you to direct log output to a variety of destinations.

## Current Publishers

- **ConsolePublisher**  
  Publishes log events to the console output asynchronously. Useful for local development, debugging, and simple diagnostics.

- **EventSourcePublisher**  
  Publishes log events using .NET's `EventSource` API. This enables integration with Event Tracing for Windows (ETW) and allows you to leverage powerful diagnostics tools and EventListeners for real-time log consumption, performance monitoring, and advanced telemetry scenarios.

## Extensibility

The publisher model allows you to easily add new log destinations by implementing the `IPublisher` interface. Future publishers could include:

- **KafkaPublisher**: Send log events to Apache Kafka for distributed processing.
- **AzureEventBusPublisher**: Publish logs to Azure Event Hubs or Service Bus for cloud-scale analytics.
- **FilePublisher**: Write logs to files for archival or batch processing.

By injecting different publishers into the logger, you can adapt your logging pipeline to meet evolving operational and business requirements without changing application code.

## Power of EventSource and EventListeners

Integrating `EventSourcePublisher` into your logging infrastructure unlocks advanced capabilities:

- **ETW Integration**: Log events can be consumed by ETW, enabling high-performance, low-overhead tracing on Windows.
- **EventListeners**: Attach custom `EventListener` implementations to process, filter, or forward log events in real time.
- **Tooling Support**: Use tools like PerfView, Windows Performance Recorder, or Azure Monitor to analyze and visualize log data.
- **Structured Logging**: EventSource supports strongly-typed, structured events, making logs more useful for diagnostics and analytics.

## Example: Injecting a Publisher

You can configure which publisher to use by registering it in your DI container and passing it to your logger implementation:

```csharp
// Register a publisher (e.g., EventSourcePublisher)
services.AddSingleton<IPublisher>(EventSourcePublisher.Log);

// Register the logger provider with the publisher
services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.AddFaultAnalysisLogger(configuration);
});
```

This design makes it easy to swap out or combine publishers as your logging needs evolve.

---

## Related Documentation

- For the publisher contract, see [../Contracts/README.md](../Contracts/README.md)
- For integration with the logging system, see [../Application/README.md](../Application/README.md)

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## License

This project is licensed under the MIT License.

---

## Contacts

For questions or support, please contact the project maintainers.