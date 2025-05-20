using Core.Configuration;
using Loggers.Application;
using Loggers.Contracts;
using Loggers.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Loggers.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IServiceCollection"/> to configure and initialize logging services,
    /// including custom <see cref="ApplicationLogProvider"/> registration and binding of <see cref="AiEventSettings"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Initializes and configures logging services for the application.
        /// Registers the <see cref="ApplicationLogProvider"/> with the logging system,
        /// applies logging configuration from the provided <paramref name="configuration"/>,
        /// and binds <see cref="AiEventSettings"/> from configuration.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add logging services to.</param>
        /// <param name="configuration">The application configuration containing logging and custom settings.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> with logging configured.</returns>
        public static IServiceCollection InitializeLogging(this IServiceCollection services, IConfiguration configuration)
        {
            // Register ApplicationLogProvider with the logging system
            services.AddLogging((loggingBuilder) =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddFaultAnalysisLogger(configuration);
            });
            return services;
        }

        /// <summary>
        /// Adds the Fault Analysis Logger to the logging system, configuring it with the specified settings.
        /// </summary>
        /// <remarks>This method registers the <see cref="ApplicationLogProvider"/> with the logging
        /// system. It clears any existing logging providers, applies logging configuration from the "Logging" section
        /// of the provided <paramref name="configuration"/>, and initializes the <see cref="ApplicationLogProvider"/>
        /// with dependencies such as <see cref="IFaultAnalysisService"/> and <see cref="IPublisher"/> if they are
        /// available in the service container.</remarks>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> used to configure logging services.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance containing the logging and application settings.</param>
        /// <returns>The <see cref="ILoggingBuilder"/> instance for chaining additional logging configurations.</returns>
        public static ILoggingBuilder AddFaultAnalysisLogger(this ILoggingBuilder builder, IConfiguration configuration)
        {
            // Ensure ApplicationLogProvider is registered only once
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ApplicationLogProvider>(sp =>
            {
                // Bind settings from configuration
                var settings = configuration.GetSection(nameof(AiEventSettings)).Get<AiEventSettings>() ?? new AiEventSettings();

                // Use a factory for log events (customize as needed)
                Func<ILogEvent> logEventFactory = () => new OtelLogEvents();

                // Resolve optional dependencies
                var publisher = sp.GetService<IPublisher>();
                var faultAnalysis = sp.GetService<IFaultAnalysisService>();
                return new ApplicationLogProvider(settings, logEventFactory, publisher, faultAnalysis);
            }));

            // Add Microsoft Logging configuration from appsettings.json
            builder.AddConfiguration(configuration.GetSection("Logging"));

            return builder;
        }
    }
}
