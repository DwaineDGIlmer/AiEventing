using Core.Configuration;
using Core.Contracts;
using Core.Serializers;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Extensions;

/// <summary>
/// Provides extension methods for configuring and initializing services in an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>This static class includes methods for setting up application services, configuring HTTP clients with
/// resilience policies, and binding application settings from a configuration source. It is designed to streamline the
/// initialization of services and ensure consistent configuration across the application.</remarks>
public static class ServiceCollectionExtensions
{
    private const string ResilientPolicies = "ResilientHttpPolicies";
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
            return GetAiEventSettings(configuration);
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

        // Override settings with environment variables if they exist
        settings.OpenAiApiKey = settings.OpenAiEnabled ?
            Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? settings.OpenAiApiKey.IsNullThrow("Requires OpenAi API key.") :
            string.Empty;
        settings.OpenAiApiUrl = settings.OpenAiEnabled ?
            Environment.GetEnvironmentVariable("OPENAI_API_URL") ?? settings.OpenAiApiUrl.IsNullThrow("Requires OpenAi API URL.") :
            string.Empty;
        settings.OpenAiModel = settings.OpenAiEnabled ?
            Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? settings.OpenAiModel.IsNullThrow("Requires OpenAi model name.") :
            string.Empty;
        settings.RcaServiceUrl = settings.RcaServiceEnabled ?
            Environment.GetEnvironmentVariable("RCASERVICE_API_URL") ?? settings.RcaServiceUrl.IsNullThrow("Requires RCA service URL.") :
            string.Empty;
        settings.RcaServiceApiKey = settings.RcaServiceEnabled ?
            Environment.GetEnvironmentVariable("RCASERVICE_API_KEY") ?? settings.RcaServiceApiKey.IsNullThrow("Requires RCA Service key.") :
            string.Empty;

        // Create a resilient HTTP for the OpenAi http client using Polly for retries and circuit breaker
        if (settings.OpenAiEnabled)
        {
            Logger.LogInformation("OpenAI functionality is enabled. API Key");

            var openAiClient = settings.OpenAiClient.IsNullThrow("Requires resilient factory name.");
            services.AddResilientHttpClient(configuration, openAiClient, null, (client) =>
            {
                var fullUri = new Uri(settings.OpenAiApiUrl.IsNullThrow("Requires OpenAi URL."));
                client.BaseAddress = new Uri(fullUri.GetLeftPart(UriPartial.Authority));
            });
        }

        // Create a resilient HTTP for the OpenAi http client using Polly for retries and circuit breaker
        if (settings.RcaServiceEnabled)
        {
            Logger.LogInformation("RCA Service functionality is enabled. API Key");

            var rcaClient = settings.RcaServiceClient.IsNullThrow("Requires resilient factory name.");
            services.AddResilientHttpClient(configuration, rcaClient, null, (client) =>
            {
                var fullUri = new Uri(settings.RcaServiceUrl.IsNullThrow("Requires RCA service URL."));
                client.BaseAddress = new Uri(fullUri.GetLeftPart(UriPartial.Authority));
            });
        }

        // Create the FaultAnalysisService using the resilient HTTP factory
        services.AddSingleton<IFaultAnalysisService, FaultAnalysisService>(sp =>
        {
            return new FaultAnalysisService(sp.GetRequiredService<IHttpClientFactory>(), settings);
        });

        return services;
    }

    /// <summary>
    /// Adds a resilient HTTP factory using Polly for retries and circuit breaker.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind settings and retrieve configuration values.</param>
    /// <param name="clientName">The logical name for the HTTP factory.</param>
    /// <param name="policyName">The resilent policy name, if not specified the default StandardResilience will be used.</param>
    /// <param name="configureClient">Delegate used to configure factory.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="clientName"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="clientName"/> is empty or whitespace.</exception>
    public static IServiceCollection AddResilientHttpClient(
        this IServiceCollection services,
        IConfiguration configuration,
        string clientName,
        string? policyName = null,
        Action<HttpClient>? configureClient = null)
    {
        services.IsNullThrow();
        configuration.IsNullThrow();
        clientName.IsNullThrow();

        var builder = configureClient.IsNotNull() ?
            services.AddHttpClient(clientName, configureClient!) :
            services.AddHttpClient(clientName);

        // No need to add resilience policies if no policy name is specified
        if (!policyName.IsNullOrEmpty())
        {
            // If no policy name is specified, use the standard resilience handler
            var timeout = GetResilientHttpPolicy(configuration).HttpTimeout;
            builder.AddBasicResilienceHandler(configuration, timeout);
            return services;
        }

        var policies = GetResilencyPolicies(configuration);
        var selectedPolicy = policies.Values.FirstOrDefault(p =>
            p.PolicyName.Equals(policyName, StringComparison.OrdinalIgnoreCase));

        if (selectedPolicy is null ||
            selectedPolicy.UseStandardResilience ||
            selectedPolicy.PolicyName != policyName ||
            selectedPolicy.HttpClientName != clientName)
        {
            var timeout = GetResilientHttpPolicy(configuration).HttpTimeout;
            builder.AddBasicResilienceHandler(configuration, timeout);
            return services;
        }

        if (selectedPolicy.HttpTimeout > 0)
        {
            builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(selectedPolicy.HttpTimeout)));
        }

        var resilentPolicy = GetResilencyPolicies(configuration)
            .Values
            .Where(p => p.HttpClientName.Equals(clientName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.PolicyOrder)
            .ToList();

        foreach (var policy in resilentPolicy)
        {
            Logger.LogInformation("Adding policy '{PolicyName}' (Order: {PolicyOrder}) to factory '{ClientName}'.", policy.PolicyName, policy.PolicyOrder, clientName);

            if (policy.RetryPolicy?.Enabled == true)
                builder.AddPolicyHandler(GetRetryPolicy(policy.RetryPolicy));
            if (policy.CircuitBreakerPolicy?.Enabled == true)
                builder.AddPolicyHandler(GetCircuitBreakerPolicy(policy.CircuitBreakerPolicy));
            if (policy.BulkheadPolicy?.Enabled == true)
                builder.AddPolicyHandler(GetBulkheadPolicy(policy.BulkheadPolicy));
        }
        return services;
    }

    /// <summary>
    /// Configures a basic resilience handler for the specified <see cref="IHttpClientBuilder"/>.
    /// </summary>
    /// <remarks>This method adds a standard resilience handler to the HTTP client, including timeout and
    /// circuit breaker strategies. The resilience handler ensures that requests are retried and failures are managed
    /// according to the specified timeout. The circuit breaker strategy is configured to trip when the failure ratio
    /// exceeds 50% within a sampling duration.</remarks>
    /// <param name="builder">The <see cref="IHttpClientBuilder"/> to configure.</param>
    /// <param name="configuration">The application configuration used to retrieve settings for the resilience handler.</param>
    /// <param name="timeout">The timeout value, in seconds, for individual requests and resilience strategies. If the value is less than or
    /// equal to 0, a default timeout of 30 seconds is used.</param>
    /// <returns>The configured <see cref="IHttpClientBuilder"/> instance.</returns>
    public static IHttpClientBuilder AddBasicResilienceHandler(this IHttpClientBuilder builder, IConfiguration configuration, int timeout)
    {
        builder.AddStandardResilienceHandler((pol) =>
        {
            pol.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(timeout > 0 ? timeout : 30)
            };
            pol.AttemptTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(timeout > 0 ? timeout : 30)
            };
            pol.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5, // 50% failure threshold
                SamplingDuration = TimeSpan.FromSeconds((timeout > 0 ? timeout : 30) * 2),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(timeout > 0 ? timeout * 2 : 60)
            };
        });
        return builder;
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
    /// Creates and returns a <see cref="ResilientHttpPolicy"/> instance configured using the provided <see
    /// cref="IConfiguration"/>.
    /// </summary>
    /// <param name="configuration">The configuration source containing the settings for the <see cref="ResilientHttpPolicy"/>.</param>
    /// <returns>A <see cref="ResilientHttpPolicy"/> instance populated with settings from the specified configuration.</returns>
    public static ResilientHttpPolicy GetResilientHttpPolicy(IConfiguration configuration)
    {
        configuration.IsNullThrow();

        // Get the "AiEventSettings" section from configuration
        var settingsSection = configuration.GetSection(nameof(ResilientHttpPolicy));
        var settings = new ResilientHttpPolicy();
        settingsSection.Bind(settings);

        return settings;
    }

    /// <summary>
    /// Retrieves a collection of resiliency policies defined in the application's configuration.
    /// </summary>
    /// <remarks>This method reads the configuration section specified by the constant
    /// <c>ResilientPolicies</c> and binds each child section to a <see cref="ResilientHttpPolicy"/> object. Policies
    /// without a valid <c>PolicyName</c> are skipped, and a warning is logged.</remarks>
    /// <param name="configuration">The configuration instance containing the resiliency policies section. Must not be <see langword="null"/>.</param>
    /// <returns>A dictionary where the keys are policy names and the values are <see cref="ResilientHttpPolicy"/> objects. If
    /// the resiliency policies section is not found in the configuration, an empty dictionary is returned.</returns>
    internal static IDictionary<string, ResilientHttpPolicy> GetResilencyPolicies(IConfiguration configuration)
    {
        configuration.IsNullThrow();

        var policies = new Dictionary<string, ResilientHttpPolicy>();
        var policiesSection = configuration.GetSection(ResilientPolicies);
        if (!policiesSection.Exists())
        {
            Logger.LogInformation("Resilient policies section '{Section}' not found in configuration.", ResilientPolicies);
            return policies;
        }

        var policyList = policiesSection.GetChildren();
        foreach (var policySection in policyList)
        {
            var policy = new ResilientHttpPolicy();
            policySection.Bind(policy);

            if (string.IsNullOrWhiteSpace(policy.PolicyName))
            {
                Logger.LogWarning("A ResilientHttpPolicy entry is missing a PolicyName. Skipping this entry.");
                continue;
            }

            policies[policy.PolicyName] = policy;
        }

        return policies;
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