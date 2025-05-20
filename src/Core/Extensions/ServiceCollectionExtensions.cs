using Core.Configuration;
using Core.Serializers;
using Core.Services;
using Loggers.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Extensions;

/// <summary>
/// Provides extension methods for configuring and initializing services in an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>This static class includes methods for setting up application services, configuring HTTP clients with
/// resilience policies, and binding application settings from a configuration source. It is designed to streamline the
/// initialization of services and ensure consistent configuration across the application.</remarks>
public static class ServiceCollectionExtensions
{
    internal static ILogger Logger { get; } = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    }).CreateLogger(nameof(ServiceCollectionExtensions));

    /// <summary>
    /// Configures and initializes services for the application by binding settings from the provided configuration
    /// and setting up JSON serializer options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind settings and retrieve configuration values.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the configured services.</returns>
    public static IServiceCollection InitializeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.IsNullThrow();
        configuration.IsNullThrow();

        services.AddService(sp =>
        {
            var settings = new AiEventSettings();
            configuration.Bind(settings);
            return settings;
        });

        var settings = GetAiEventSettings(configuration);
        if (JsonConvertService.Instance.IsNull())
        {
            // Initialize the JsonConvertService with settings from configuration          
            JsonConvertService.Initialize(new JsonSerializerOptions()
            {
                WriteIndented = settings.WriteIndented,
                DefaultIgnoreCondition = settings.DefaultIgnoreCondition,
                Encoder = settings.UnsafeRelaxedJsonEscaping ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping : null
            });

            // Add JsonConvertService to the service collection 
            services.AddService(sp =>
            {
                return JsonConvertService.Instance!;
            });
        }
        else
        {
            Logger.LogWarning("JsonConvertService instance already initialized. Skipping initialization.");
        }

        // Create a resilient HTTP client using Polly for retries and circuit breaker
        var clientName = settings.FaultServiceClientName.IsNullThrow("Requires resilient client name.");
        services.AddResilientHttpClient(configuration, clientName);

        // Create a resilient HTTP client using the specified name
        var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(clientName);
        var apiKey = Environment.GetEnvironmentVariable("AI_API_KEY") ?? settings.ApiKey.IsNullThrow("Requires API key.");
        var apiUrl = Environment.GetEnvironmentVariable("AI_API_URL") ?? settings.ApiUrl.IsNullThrow("Requires API URL.");
        var model = Environment.GetEnvironmentVariable("AI_MODEL") ?? settings.Model.IsNullThrow("Requires model name.");

        // Create the FaultAnalysisService using the resilient HTTP client
        services.AddService<IFaultAnalysisService>(sp =>
        {
            return new FaultAnalysisService(client, model, apiKey, apiUrl);
        });

        return services;
    }

    /// <summary>
    /// Adds a resilient HTTP client using Polly for retries and circuit breaker.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind settings and retrieve configuration values.</param>
    /// <param name="clientName">The logical name for the HTTP client.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="clientName"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="clientName"/> is empty or whitespace.</exception>
    public static IServiceCollection AddResilientHttpClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string clientName)
    {
        services.IsNullThrow();
        configuration.IsNullThrow();
        clientName.IsNullThrow();

        var settings = GetAiEventSettings(configuration);
        services.AddHttpClient(clientName)
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(settings.HttpTimeout)))
            .AddPolicyHandler(GetBulkheadPolicy(settings.BulkheadSettings))
            .AddPolicyHandler(GetRetryPolicy(settings.RetrySettings))
            .AddPolicyHandler(GetCircuitBreakerPolicy(settings.CircuitBreakerSettings));
        return services;
    }

    /// <summary>
    /// Retrieves the AI event settings from the specified configuration source.
    /// </summary>
    /// <remarks>This method retrieves settings from the "AiEventSettings" section of the configuration source
    /// and binds them to an <see cref="AiEventSettings"/> object. Additionally, it determines the minimum log level
    /// from the "Logging:LogLevel:Default" configuration value.</remarks>
    /// <param name="configuration">The configuration source from which to retrieve the AI event settings.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <returns>An <see cref="AiEventSettings"/> object populated with values from the configuration source. The <see
    /// cref="AiEventSettings.MinLogLevel"/> property is set based on the "Logging:LogLevel:Default" configuration
    /// value, defaulting to <see cref="LogLevel.Information"/> if the value is not specified or invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is <see langword="null"/>.</exception>
    public static AiEventSettings GetAiEventSettings(IConfiguration configuration)
    {
        configuration.IsNullThrow();

        // Get the "Logging" section from configuration
        var loggingSection = configuration.GetSection("Logging");

        // Get the "LogLevel" section from configuration
        var logLevel = loggingSection.GetSection("LogLevel")["Default"];
        var logLevelValue = Enum.TryParse(logLevel, out LogLevel parsedLogLevel) ? parsedLogLevel : LogLevel.Information;
        LogLevel minLevel = logLevelValue;

        // Get the "AiEventSettings" section from configuration
        var settingsSection = configuration.GetSection(nameof(AiEventSettings));
        var settings = new AiEventSettings();
        settingsSection.Bind(settings);
        settings.MinLogLevel = minLevel;

        return settings;
    }

    /// <summary>
    /// Adds the specified service to the service collection.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services">The service collection to add the service to.</param>
    /// <returns>The updated service collection.</returns>
    internal static IServiceCollection AddService<T>(this IServiceCollection services)
        where T : class
    {
        services.TryAddSingleton<T>();
        return services;
    }

    /// <summary>
    /// Adds a singleton service of the type specified by <typeparamref name="T"/> to the 
    /// <see cref="IServiceCollection"/> using the specified factory function.
    /// </summary>
    /// <typeparam name="T">The type of the service to add.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="factory">
    /// A factory function that creates an instance of <typeparamref name="T"/>. 
    /// The factory function takes an <see cref="IServiceProvider"/> as a parameter.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
    internal static IServiceCollection AddService<T>(this IServiceCollection services, Func<IServiceProvider, T> factory)
       where T : class
    {
        services.TryAddSingleton(factory);
        return services;
    }

    /// <summary>
    /// Creates and returns an asynchronous bulkhead policy for <see cref="HttpResponseMessage"/> requests,
    /// optionally wrapping it with a fallback policy that returns a 503 ServiceUnavailable response when the bulkhead limit is reached.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="BulkheadSettings"/> that determine whether the bulkhead is enabled and configure its parameters,
    /// such as maximum parallelization and maximum queuing actions.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncPolicy{HttpResponseMessage}"/> representing the configured bulkhead policy.
    /// If the bulkhead is disabled, a no-op policy is returned. If enabled, a bulkhead policy is wrapped with a fallback
    /// that returns a 503 ServiceUnavailable response when the bulkhead is full.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="settings.MaxParallelization"/> or <paramref name="settings.MaxQueuingActions"/> is less than or equal to zero.</exception>
    internal static IAsyncPolicy<HttpResponseMessage> GetBulkheadPolicy(BulkheadSettings settings)
    {
        if (!settings.Enabled)
            return Policy.NoOpAsync<HttpResponseMessage>();

        var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization: settings.MaxParallelization,
            maxQueuingActions: settings.MaxQueuingActions
        );

        var fallbackPolicy = Policy<HttpResponseMessage>
            .Handle<Polly.Bulkhead.BulkheadRejectedException>()
            .FallbackAsync(
                fallbackAction: (ct) =>
                {
                    Logger.LogWarning("Bulkhead limit reached. Request is being rejected and will receive 503 ServiceUnavailable.");
                    return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.ServiceUnavailable));
                }
            );

        return fallbackPolicy.WrapAsync(bulkheadPolicy);
    }

    /// <summary>
    /// Creates an asynchronous retry policy for HTTP requests based on the provided <see cref="RetrySettings"/>.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="RetrySettings"/> object containing configuration for retry behavior, including whether retries are enabled,
    /// the maximum number of retries, base delay, jitter, and maximum delay.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncPolicy{HttpResponseMessage}"/> that retries failed HTTP requests according to the specified settings,
    /// or a no-op policy if retries are disabled.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="settings.MaxRetryCount"/> is less than or equal to zero.</exception>
    internal static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(RetrySettings settings)
    {
        if (!settings.Enabled)
            return Policy.NoOpAsync<HttpResponseMessage>();

        var random = new Random();

        return Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                settings.MaxRetryCount,
                retryAttempt =>
                {
                    var baseDelayMs = settings.Delay * 1000;
                    var jitterMs = random.Next(0, settings.Jitter * 1000);
                    var totalDelayMs = baseDelayMs + jitterMs;
                    var cappedDelayMs = Math.Min(totalDelayMs, settings.MaxDelay * 1000);
                    return TimeSpan.FromMilliseconds(cappedDelayMs);
                },
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Logger.LogWarning(
                        outcome.Exception,
                        "Retry {RetryCount} encountered an error: {ExceptionMessage}. Waiting {Delay} before next retry.",
                        retryCount,
                        outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase,
                        timespan.TotalMilliseconds
                    );
                }
            );
    }

    /// <summary>
    /// Creates and returns an asynchronous circuit breaker policy for HTTP responses based on the provided settings.
    /// </summary>
    /// <param name="settings">
    /// The <see cref="CircuitBreakerSettings"/> object containing configuration for the circuit breaker,
    /// including whether it is enabled, the failure threshold, and the duration of the break.
    /// </param>
    /// <returns>
    /// An <see cref="IAsyncPolicy{HttpResponseMessage}"/> representing the configured circuit breaker policy.
    /// If the circuit breaker is disabled, a no-op policy is returned.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="settings.FailureThreshold"/> is less than or equal to zero.</exception>
    internal static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(CircuitBreakerSettings settings)
    {
        if (!settings.Enabled)
            return Policy.NoOpAsync<HttpResponseMessage>();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: settings.FailureThreshold,
                durationOfBreak: TimeSpan.FromSeconds(settings.DurationOfBreak),
                onBreak: (outcome, breakDelay, context) =>
                {
                    Logger.LogWarning(
                        outcome.Exception,
                        "Circuit breaker opened for {BreakDelay}ms due to: {Reason}",
                        breakDelay.TotalMilliseconds,
                        outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase
                    );
                },
                onReset: (context) =>
                {
                    Logger.LogInformation("Circuit breaker reset. Normal operation resumed.");
                },
                onHalfOpen: () =>
                {
                    Logger.LogInformation("Circuit breaker is half-open. Testing for recovery.");
                }
            );
    }

    /// <summary>
    /// Retrieves the <see cref="JsonIgnoreCondition"/> value from the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration instance to retrieve the setting from.</param>
    /// <returns>The <see cref="JsonIgnoreCondition"/> value specified in the configuration.  If the configuration does not
    /// specify a valid value, <see cref="JsonIgnoreCondition.Never"/> is returned by default.</returns>
    /// <remarks>Internal for unit testing.</remarks>
    internal static JsonIgnoreCondition GetJsonIgnoreCondition(IConfiguration configuration)
    {
        // Validate the configuration for AiEventSettings
        var defaultIgnoreConditionString = configuration[$"{nameof(AiEventSettings)}:{nameof(AiEventSettings.DefaultIgnoreCondition)}"];
        JsonIgnoreCondition defaultIgnoreCondition = JsonIgnoreCondition.Never;
        if (!string.IsNullOrEmpty(defaultIgnoreConditionString) &&
            Enum.TryParse(defaultIgnoreConditionString, out JsonIgnoreCondition parsedCondition))
        {
            defaultIgnoreCondition = parsedCondition;
        }
        return defaultIgnoreCondition;
    }
}