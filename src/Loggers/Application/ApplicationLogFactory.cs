using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using Loggers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Loggers.Application;

/// <summary>
/// Provides a factory for creating and managing loggers in an application, including support for custom logger
/// providers and integration with external systems.
/// </summary>
/// <remarks>This factory allows the creation of <see cref="ILogger"/> instances for specific categories, the
/// registration of custom <see cref="ILoggerProvider"/> implementations, and optional integration with external
/// publishers and fault analysis services. It is designed to be used as a central logging facility within an
/// application.</remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationLogFactory"/> class with the specified settings, log
/// event factory, and optional services.
/// </remarks>
/// <param name="settings">The settings used to configure the application log factory. This parameter cannot be <see langword="null"/>.</param>
/// <param name="factory">A delegate that creates instances of <see cref="ILogEvent"/>. This parameter cannot be <see langword="null"/>.</param>
/// <param name="publisher">An optional publisher used to send log events to external systems. If <see langword="null"/>, no publishing will
/// occur.</param>
/// <param name="faultAnalysisService">An optional service for analyzing faults in the application. If <see langword="null"/>, fault analysis will not
/// be performed.</param>
public sealed class ApplicationLogFactory(
    IOptions<AiEventSettings> settings,
    Func<ILogEvent> factory,
    IPublisher? publisher = null,
    IFaultAnalysisService? faultAnalysisService = null) : ILoggerFactory
{
    #region Internal Fields
    /// <summary>
    /// Gets a <see cref="Publisher"/> instance used to direct tracing or debugging output to the console.
    /// </summary>
    internal IPublisher? Publisher { get; set; } = publisher;

    /// <summary>
    /// Gets or sets the service responsible for analyzing faults in the system.
    /// </summary>
    internal IFaultAnalysisService? FaultAnalysisService { get; set; } = faultAnalysisService;

    /// <summary>
    /// Gets the list of registered <see cref="ILoggerProvider"/> instances.
    /// </summary>
    internal IList<ILoggerProvider> Providers { get; } = [];

    /// <summary>
    /// Gets the dictionary of created <see cref="ILogger"/> instances, keyed by category name.
    /// </summary>
    internal ConcurrentDictionary<string, ILogger> Loggers { get; } = new();

    /// <summary>
    /// Gets the factory method used to create log events.
    /// </summary>  
    internal Func<ILogEvent> LogEventFactory { get; } = factory.IsNullThrow();

    /// <summary>
    /// The settings used for configuring the logger. 
    /// </summary>
    internal IOptions<AiEventSettings> Settings { get; } = settings.IsNullThrow();
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds a logger provider to the factory.
    /// </summary>
    /// <param name="provider">The <see cref="ILoggerProvider"/> to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="provider"/> is <c>null</c>.</exception>
    public void AddProvider(ILoggerProvider provider)
    {
        provider.IsNullThrow();

        if (Providers.Contains(provider))
            return;

        Providers.Add(provider);
    }

    /// <summary>
    /// Creates or retrieves a logger for the specified category name.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>An <see cref="ILogger"/> instance for the specified category.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return Loggers.GetOrAdd(categoryName, name =>
        {
            return new ApplicationLogger(name, Settings, LogEventFactory, Publisher, FaultAnalysisService);
        });
    }

    /// <summary>
    /// Disposes the factory and all registered logger providers, and clears all created loggers.
    /// </summary>
    public void Dispose()
    {
        foreach (var provider in Providers)
        {
            provider.Dispose();
        }
        Providers.Clear();
        Loggers.Clear();

        // Suppress finalization to comply with CA1816
        GC.SuppressFinalize(this);
    }
    #endregion
}