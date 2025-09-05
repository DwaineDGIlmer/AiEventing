using Azure.Storage.Blobs;
using Core.Caching;
using Core.Configuration;
using Core.Constants;
using Core.Contracts;
using Core.Serializers;
using Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http.Resilience;
using OpenAI;
using Polly;
using Polly.Extensions.Http;

namespace Core.Extensions;

/// <summary>
/// Provides extension methods for configuring and initializing services in an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>This static class includes methods for setting up application services, configuring HTTP clients with
/// resilience policies, and binding application aiEventSettings from a configuration source. It is designed to streamline the
/// initialization of services and ensure consistent configuration across the application.</remarks>
public static class ServiceCollectionExtensions
{
    private const string ResilientPolicies = "ResilientHttpPolicies";
    internal static ILogger Logger { get; } = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    }).CreateLogger(nameof(ServiceCollectionExtensions));

    /// <summary>
    /// Configures and initializes services for the application by binding aiEventSettings from the provided configuration
    /// and setting up JSON serializer options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind aiEventSettings and retrieve configuration values.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the configured services.</returns>
    public static IServiceCollection InitializeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.IsNullThrow();
        configuration.IsNullThrow();

        // Add the cache service to the service collection
        services.AddCacheService(configuration);

        // Bind the OpenAiSettings from configuration
        services.Configure<OpenAiSettings>(options =>
        {
            // Bind configuration values to options
            configuration.GetSection(nameof(AiEventSettings)).Bind(options);

            // Apply environment variable and default overrides
            if (options.IsEnabled)
            {
                options.BaseAddress = Environment.GetEnvironmentVariable("AI_API_BASE_ADDRESS") ?? options.BaseAddress ?? Defaults.OpenAiABaseAddress;
                options.Endpoint = Environment.GetEnvironmentVariable("AI_API_ENDPOINT") ?? options.Endpoint ?? Defaults.OpenAiEndpoint;
                options.Model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? options.Model;
                options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? options.ApiKey ?? string.Empty;
            }
        });

        // Bind the AiEventSettings from configuration
        services.Configure<AiEventSettings>(options =>
        {
            // Bind configuration values to options
            configuration.GetSection(nameof(AiEventSettings)).Bind(options);
            options.RcaServiceUrl = Environment.GetEnvironmentVariable(DefaultConstants.RCASERVICE_API_URL) ?? options.RcaServiceUrl;
            options.RcaServiceApiKey = Environment.GetEnvironmentVariable(DefaultConstants.RCASERVICE_API_KEY) ?? options.RcaServiceApiKey;
        });

        // Add JsonConvertService to the service collection if it has not been initialized yet
        var aiEventSettings = GetAiEventSettings(configuration);
        if (JsonConvertService.Instance.IsNull())
        {
            // Initialize the JsonConvertService with aiEventSettings from configuration          
            JsonConvertService.Initialize(new JsonSerializerOptions()
            {
                WriteIndented = aiEventSettings.WriteIndented,
                DefaultIgnoreCondition = aiEventSettings.DefaultIgnoreCondition,
                Encoder = aiEventSettings.UnsafeRelaxedJsonEscaping ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping : null
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

        // Create a resilient HTTP for the OpenAi http client using Polly for retries and circuit breaker
        var openAiSettings = GetOpenAiSettings(configuration);
        if (openAiSettings.IsEnabled)
        {
            Logger.LogInformation("OpenAI functionality is enabled. API Key");

            var openAiClient = openAiSettings.HttpClientName.IsNullThrow("Requires resilient factory name.");
            services.AddResilientHttpClient(configuration, openAiClient, null, (client) =>
            {
                client.BaseAddress = new Uri(openAiSettings.BaseAddress);
            });
        }

        // Create a resilient HTTP for the OpenAi http client using Polly for retries and circuit breaker
        if (aiEventSettings.FaultServiceEnabled)
        {
            Logger.LogInformation("RCA Service functionality is enabled. API Key");
            services.AddSingleton<IFaultAnalysisService, FaultAnalysisService>(sp =>
            {
                return new FaultAnalysisService(
                    sp.GetRequiredService<IOpenAiChatService>(),
                    sp.GetRequiredService<IOptions<AiEventSettings>>());
            });
        }

        #region OpenAI Configuration and DI 
        services.Configure<OpenAiSettings>(configuration.GetSection(nameof(OpenAiSettings)));
        services.TryAddSingleton<IOpenAiChatService, OpenAiChatService>();
        services.AddSingleton(sp =>
        {
            var apiKey = openAiSettings.ApiKey ?? Environment.GetEnvironmentVariable(DefaultConstants.OPENAI_API_KEY).IsNullThrow();
            return new OpenAIClient(apiKey);
        });
        services.TryAddSingleton(sp =>
        {
            var apiKey = openAiSettings.ApiKey ?? Environment.GetEnvironmentVariable(DefaultConstants.OPENAI_API_KEY).IsNullThrow();
            var cacheService = GetFileCaching(configuration, sp, nameof(OpenAiEmbeddingService));
            var service = sp.GetRequiredService<OpenAIClient>();
            var settings = sp.GetRequiredService<IOptions<OpenAiSettings>>();
            var logger = sp.GetRequiredService<ILogger<OpenAiChatService>>();
            return new OpenAiChatService(service, settings, cacheService, logger);
        });

        // Add the OpenAi embedding service to the service collection for Vector DB embedding
        services.TryAddSingleton<IEmbeddingService>(sp =>
        {
            var caching = GetFileCaching(configuration, sp, nameof(OpenAiEmbeddingService));
            var embLogger = sp.GetRequiredService<ILogger<OpenAiEmbeddingService>>();
            var apiKey = openAiSettings.ApiKey ?? Environment.GetEnvironmentVariable(DefaultConstants.OPENAI_API_KEY).IsNullThrow();
            return new OpenAiEmbeddingService(new OpenAIClient(apiKey), caching);
        });
        #endregion

        return services;
    }

    /// <summary>
    /// Adds caching services to the specified <see cref="IServiceCollection"/> based on the configuration settings.
    /// </summary>
    /// <remarks>This method configures caching services based on the specified caching type in the
    /// configuration: <list type="bullet"> <item> <description>If the caching type is <see
    /// cref="Enums.CachingTypes.InMemory"/>, an in-memory caching service is registered, along with related
    /// dependencies such as <see cref="ICacheLoader"/> and <see cref="ICacheBlobClient"/>.</description> </item> <item>
    /// <description>If the caching type is <see cref="Enums.CachingTypes.FileSystem"/>, a file system-based caching
    /// service is registered, with the cache location determined by the local application data folder or the
    /// application's base directory.</description> </item> </list> The method ensures that required dependencies are
    /// properly configured and registered in the service collection.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the caching services will be added. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing the caching configuration settings. Cannot be <see
    /// langword="null"/>.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the caching services registered.</returns>
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        services.IsNullThrow(nameof(services));
        configuration.IsNullThrow(nameof(configuration));

        // Configure MemoryCacheSettings from configuration
        services.Configure<MemoryCacheSettings>(configuration.GetSection(nameof(MemoryCacheSettings)));
        services.PostConfigure<MemoryCacheSettings>(options =>
        {
            var connectionString = configuration.GetConnectionString(DefaultConstants.AzureWebJobsStorage);
            options.AccountUrl = connectionString ?? Environment.GetEnvironmentVariable(DefaultConstants.AzureWebJobsStorage) ?? options.AccountUrl;
        });

        // If the caching type is InMemory, add memory cache and related services
        var settings = GetAiEventSettings(configuration);
        if (settings.CachingType == Enums.CachingTypes.InMemory)
        {
            services.AddMemoryCache();
            services.AddCacheLoaderService(configuration);
            services.TryAddSingleton<ICacheService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<MemoryCacheService>>();
                var options = sp.GetRequiredService<IOptions<MemoryCacheSettings>>();
                var memoryCache = sp.GetRequiredService<IMemoryCache>();
                var loader = sp.GetService<ICacheLoader>();

                return new MemoryCacheService(memoryCache, options, logger, loader);
            });
        }
        if (settings.CachingType == Enums.CachingTypes.FileSystem)
        {
            services.TryAddSingleton<ICacheService>(sp =>
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                if (dir is not null && !string.IsNullOrEmpty(dir.ToString()))
                {
                    settings.CacheLocation = Path.Combine(dir.ToString()!, "cache");
                }
                else
                {
                    settings.CacheLocation = Path.Combine(AppContext.BaseDirectory, "cache");
                }
                var cacheLogger = sp.GetRequiredService<ILogger<FileCacheService>>();
                return new FileCacheService(cacheLogger, settings.CacheLocation, true);
            });
        }
        return services;
    }

    /// <summary>
    /// Adds the <see cref="BlobCachingService"/> to the service collection, enabling blob caching functionality if enabled.
    /// </summary>
    /// <remarks>This is primarily for testing as mocking the Azure BlobServiceClient is not easy.</remarks>
    /// <remarks>This method registers the <see cref="BlobCachingService"/> as a singleton implementation of
    /// <see cref="ICacheBlobClient"/>. If no <paramref name="serviceClient"/> is provided, the method creates a new
    /// <see cref="BlobServiceClient"/> using the account URL specified in the application's memory cache
    /// settings.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the service will be added.</param>
    /// <param name="configuration">The application's configuration, used to retrieve necessary settings.</param>
    /// <param name="serviceClient">An optional <see cref="BlobServiceClient"/> instance. If provided, it will be used by the caching service;
    /// otherwise, a new instance will be created using configuration settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the blob caching service registered.</returns>
    public static IServiceCollection AddCacheLoaderService(
        this IServiceCollection services,
        IConfiguration configuration,
        BlobServiceClient? serviceClient = null)
    {
        services.IsNullThrow(nameof(services));
        configuration.IsNullThrow(nameof(configuration));

        var settings = GetMemoryCacheSettings(configuration);
        bool isRegistered = services.Any(sd => sd.ServiceType == typeof(ICacheLoader));

        // Only add if not already registered and enabled in settings.
        if (settings.UseCacheLoader && !isRegistered)
        {
            // Testing use case we can pass in a mock client.
            if (serviceClient is not null)
            {
                services.AddSingleton<ICacheBlobClient, BlobCachingService>(sp =>
                {
                    var memSettings = sp.GetRequiredService<IOptions<MemoryCacheSettings>>();
                    return new BlobCachingService(serviceClient, memSettings);
                });
            }
            else
            {
                // Real use case we create the client here.
                services.AddSingleton<ICacheBlobClient, BlobCachingService>(sp =>
                {
                    var memSettings = sp.GetRequiredService<IOptions<MemoryCacheSettings>>();
                    return new BlobCachingService(new BlobServiceClient(memSettings.Value.AccountUrl), memSettings);
                });
            }
            services.AddSingleton<ICacheLoader, CacheLoaderService>();
        }
        return services;
    }

    /// <summary>
    /// Adds a resilient HTTP factory using Polly for retries and circuit breaker.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance used to bind aiEventSettings and retrieve configuration values.</param>
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

        // If the caller is configuring the client we return.
        if (configureClient is not null)
        {
            var clientBuilder = services.AddHttpClient(clientName, configureClient);
            return services;
        }

        // No need to add resilience policies if no policy name is specified
        var builder = services.AddHttpClient(clientName);
        if (policyName.IsNullOrEmpty())
        {
            // If no policy name is specified, use the standard resilience handler
            var timeout = GetResilientHttpPolicy(configuration).HttpTimeout;
            builder.AddBasicResilienceHandler(timeout);
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
            builder.AddBasicResilienceHandler(timeout);
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
    /// <param name="timeout">The timeout value, in seconds, for individual requests and resilience strategies. If the value is less than or
    /// equal to 0, a default timeout of 60 seconds is used.</param>
    /// 
    /// <returns>The configured <see cref="IHttpClientBuilder"/> instance.</returns>
    public static IHttpClientBuilder AddBasicResilienceHandler(this IHttpClientBuilder builder, int timeout)
    {
        builder.IsNullThrow();

        var effectiveTimeout = timeout > 0 ? timeout : Defaults.HttpTimeout;
        builder.AddStandardResilienceHandler(pol =>
        {
            pol.AttemptTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(effectiveTimeout)
            };
            pol.TotalRequestTimeout = new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(effectiveTimeout * 2)
            };
            pol.CircuitBreaker = new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(effectiveTimeout * 2),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(effectiveTimeout * 2)
            };
        });
        builder.ConfigureHttpClient(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(effectiveTimeout);
        });
        return builder;
    }

    /// <summary>
    /// Retrieves the AI event aiEventSettings from the specified configuration source.
    /// </summary>
    /// <remarks>This method retrieves aiEventSettings from the "AiEventSettings" section of the configuration source
    /// and binds them to an <see cref="AiEventSettings"/> object. Additionally, it determines the minimum log level
    /// from the "Logging:LogLevel:Default" configuration value.</remarks>
    /// <param name="configuration">The configuration source from which to retrieve the AI event aiEventSettings.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <returns>An <see cref="AiEventSettings"/> object populated with values from the configuration source. The <see
    /// cref="AiEventSettings.MinLogLevel"/> property is set based on the "Logging:LogLevel:Default" configuration
    /// value, defaulting to <see cref="LogLevel.Information"/> if the value is not specified or invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is <see langword="null"/>.</exception>
    public static AiEventSettings GetAiEventSettings(IConfiguration configuration)
    {
        configuration.IsNullThrow();

        var loggingSection = configuration.GetSection("Logging");
        var logLevel = loggingSection.GetSection("LogLevel")["Default"];
        var logLevelValue = Enum.TryParse(logLevel, out LogLevel parsedLogLevel) ? parsedLogLevel : LogLevel.Information;
        LogLevel minLevel = logLevelValue;

        var settingsSection = configuration.GetSection(nameof(AiEventSettings));
        var settings = new AiEventSettings();
        settingsSection.Bind(settings);
        settings.MinLogLevel = minLevel;

        settings.RcaServiceUrl = Environment.GetEnvironmentVariable(DefaultConstants.RCASERVICE_API_URL) ?? settings.RcaServiceUrl;
        settings.RcaServiceApiKey = Environment.GetEnvironmentVariable(DefaultConstants.RCASERVICE_API_KEY) ?? settings.RcaServiceApiKey;

        return settings;
    }

    /// <summary>
    /// Retrieves the memory cache settings from the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration instance containing the memory cache settings.</param>
    /// <returns>A <see cref="MemoryCacheSettings"/> object populated with values from the configuration. If the account URL is
    /// not specified in the configuration, it is set to the value of the  <see
    /// cref="DefaultConstants.AzureWebJobsStorage"/> environment variable, if available.</returns>
    public static MemoryCacheSettings GetMemoryCacheSettings(IConfiguration configuration)
    {
        configuration.IsNullThrow();

        var connectionString = configuration.GetConnectionString(DefaultConstants.AzureWebJobsStorage);
        var settingsSection = configuration.GetSection(nameof(MemoryCacheSettings));
        var settings = new MemoryCacheSettings();
        settingsSection.Bind(settings);
        settings.AccountUrl = connectionString ?? Environment.GetEnvironmentVariable(DefaultConstants.AzureWebJobsStorage) ?? settings.AccountUrl;
        return settings;
    }

    /// <summary>
    /// Used to add file caching service to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="configuration">The <see cref="IConfiguration"/>used for adding the services to.</param>
    /// <param name="sp">ServiceProvider instance to resolve dependencies.</param>
    /// <param name="name">The name of the directory to use, if null will use the default.</param>
    /// 
    /// <remarks>The name should be the class name.</remarks>
    /// <returns>IServiceCollection instance.</returns>
    internal static FileCacheService GetFileCaching(IConfiguration configuration, IServiceProvider sp, string name)
    {
        var settings = configuration.GetSettings<AiEventSettings>();
        var cacheLogger = sp.GetRequiredService<ILogger<FileCacheService>>();
        return new FileCacheService(cacheLogger, name, settings.EnableCaching);
    }

    /// <summary>
    /// Retrieves the OpenAI aiEventSettings from the specified configuration.
    /// </summary>
    /// <remarks>This method reads the OpenAI aiEventSettings from the configuration, including any overrides from
    /// environment variables. If the aiEventSettings are enabled, it ensures that required fields such as the API key and
    /// model name are populated.</remarks>
    /// <param name="configuration">The configuration source from which to retrieve the OpenAI aiEventSettings. Cannot be null.</param>
    /// <returns>An <see cref="OpenAiSettings"/> object populated with the aiEventSettings from the configuration and environment
    /// variables.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null or if required environment variables are missing when
    /// aiEventSettings are enabled.</exception>
    public static OpenAiSettings GetOpenAiSettings(IConfiguration configuration)
    {
        configuration.IsNullThrow();

        var settingsSection = configuration.GetSection(nameof(OpenAiSettings));
        var settings = new OpenAiSettings();
        settingsSection.Bind(settings);

        if (settings.IsEnabled)
        {
            settings.BaseAddress = string.IsNullOrEmpty(settings.BaseAddress) ? Defaults.OpenAiABaseAddress : settings.BaseAddress;
            settings.Endpoint = string.IsNullOrEmpty(settings.Endpoint) ? Defaults.OpenAiEndpoint : settings.Endpoint;
            settings.ApiKey = string.IsNullOrEmpty(settings.ApiKey) ?
                Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new ArgumentNullException(nameof(configuration), "Requires OpenAi API key.") : settings.ApiKey;
            settings.Model = string.IsNullOrEmpty(settings.Model) ?
                Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? throw new ArgumentNullException(nameof(configuration), "Requires OpenAi model name.") : settings.Model;
        }
        return settings;
    }

    /// <summary>
    /// Creates and returns a <see cref="ResilientHttpPolicy"/> instance configured using the provided <see
    /// cref="IConfiguration"/>.
    /// </summary>
    /// <param name="configuration">The configuration source containing the aiEventSettings for the <see cref="ResilientHttpPolicy"/>.</param>
    /// <returns>A <see cref="ResilientHttpPolicy"/> instance populated with aiEventSettings from the specified configuration.</returns>
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
    /// Retrieves and binds configuration settings to an instance of the specified type.
    /// </summary>
    /// <remarks>The method binds the configuration section named after the type <typeparamref name="T"/> to a
    /// new instance of the type. Ensure that the configuration contains a section matching the type name for successful
    /// binding.</remarks>
    /// <typeparam name="T">The type of the settings object to create and bind. Must be a reference type with a parameterless constructor.</typeparam>
    /// <param name="configuration">The configuration source from which to retrieve the settings. Cannot be <see langword="null"/>.</param>
    /// <returns>An instance of type <typeparamref name="T"/> populated with values from the configuration.</returns>
    public static T GetSettings<T>(this IConfiguration configuration) where T : class, new()
    {
        configuration.IsNullThrow(nameof(configuration), "Configuration is null.");

        var sectionKey = typeof(T).Name;
        if (string.IsNullOrWhiteSpace(sectionKey))
            throw new ArgumentException("Section key cannot be null or empty.");

        var section = configuration.GetSection(sectionKey);
        if (!section.Exists())
            throw new InvalidOperationException($"Section '{sectionKey}' not found in configuration.");

        // Create settings instance and bind configuration
        var settings = new T();
        ConfigurationBinder.Bind(section, settings, options =>
        {
            options.BindNonPublicProperties = false;
            options.ErrorOnUnknownConfiguration = true;
        });

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
    /// An <see cref="IAsyncPolicy{HttpResponseMessage}"/> that retries failed HTTP requests according to the specified aiEventSettings,
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
                        "Retry {RetryCount} encountered an error: {Message}. Waiting {Delay} before next retry.",
                        retryCount,
                        outcome.Exception?.Message ?? outcome.Result?.ReasonPhrase,
                        timespan.TotalMilliseconds
                    );
                }
            );
    }

    /// <summary>
    /// Creates and returns an asynchronous circuit breaker policy for HTTP responses based on the provided aiEventSettings.
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