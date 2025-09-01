using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using Loggers.Contracts;
using Loggers.Publishers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Loggers.Application
{
    /// <summary>
    /// Provides a logging provider implementation that creates loggers for application-specific logging.
    /// </summary>
    /// <remarks>The <see cref="ApplicationLogProvider"/> is designed to configure and manage loggers for
    /// application logging. It supports custom log event factories, optional publishers for log output, and an optional
    /// fault analysis service for diagnosing system faults. This provider is typically used in conjunction with the
    /// .NET logging infrastructure.</remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ApplicationLogProvider"/> class with the specified settings,
    /// log event factory, and optional publisher and fault analysis service.
    /// </remarks>
    /// <param name="settings">The settings used to configure the application log provider. Cannot be <see langword="null"/>.</param>
    /// <param name="factory">A factory function to create instances of <see cref="ILogEvent"/>. Cannot be <see langword="null"/>.</param>
    /// <param name="publisher">An optional publisher for log events. If <see langword="null"/>, a default <see cref="ConsolePublisher"/> is
    /// used with the polling delay specified in <paramref name="settings"/>.</param>
    /// <param name="faultAnalysis">An optional fault analysis service to assist with fault diagnostics. Can be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> or <paramref name="factory"/> is <see langword="null"/>.</exception>"
    sealed public class ApplicationLogProvider(
        IOptions<AiEventSettings> settings,
        Func<ILogEvent> factory,
        IPublisher? publisher = null,
        IFaultAnalysisService? faultAnalysis = null) : ILoggerProvider
    {
        #region Internal Fields
        /// <summary>
        /// The settings used for configuring the logger. 
        /// </summary>
        internal IOptions<AiEventSettings> Settings { get; } = settings.IsNullThrow();


        /// <summary>
        /// Gets the factory method used to create log events.
        /// </summary>  
        internal Func<ILogEvent> LogEventFactory { get; } = factory.IsNullThrow();

        /// <summary>
        /// Gets a <see cref="Publisher"/> instance used to direct tracing or debugging output to the console.
        /// </summary>
        internal IPublisher Publisher { get; set; } = publisher ?? new ConsolePublisher(settings.Value.PollingDelay);

        /// <summary>
        /// Gets or sets the service responsible for analyzing faults in the system.
        /// </summary>
        internal IFaultAnalysisService? FaultAnalysisService { get; set; } = faultAnalysis;
        #endregion

        #region Public Constructors
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance for the specified category.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>An <see cref="ILogger"/> instance configured for the specified category.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new ApplicationLogger(categoryName, Settings, LogEventFactory, Publisher, FaultAnalysisService);
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ApplicationLogProvider"/>.
        /// </summary>
        public void Dispose()
        {
            // No resources to dispose
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
