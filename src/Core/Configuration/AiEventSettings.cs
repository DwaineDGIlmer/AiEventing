using Core.Constants;
using Core.Enums;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Core.Configuration;

/// <summary>
/// Settings used across the eventing service.
/// </summary>
public class AiEventSettings
{
    /// <summary>
    /// Gets or sets the unique identifier for the application.
    /// </summary>
    public string ApplicationId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the component.
    /// </summary>
    public string ComponentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the environment in which the application is running.
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the application or component.
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether all caching is disabled or not.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the type of caching mechanism to be used.
    /// </summary>
    public CachingTypes CachingType { get; set; } = CachingTypes.InMemory;

    /// <summary>
    /// Gets or sets the name of the Azure Table used for storage operations.
    /// </summary>
    public string? AzureTableName { get; set; } = "appdata";

    /// <summary>
    /// Gets or sets the location where cached data is stored.
    /// </summary>
    public string? CacheLocation { get; set; } = null;

    /// <summary>
    /// Gets or sets the unique identifier for the deployment.
    /// </summary>
    public string DeploymentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the RCS Services feature is enabled.
    /// </summary>
    public bool RcaServiceEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the Logging feature is enabled.
    /// </summary>
    public bool LoggingEnabled { get; set; } = true;

    /// <summary>
    /// The name of the RCS service REST service client used for making requests.
    /// </summary>
    public string RcaServiceClient { get; set; } = nameof(RcaServiceClient);

    /// <summary>
    /// Gets or sets the base URL of the API used for making requests to AI service.
    /// </summary>
    public string RcaServiceUrl { get; set; } = string.Empty;

    /// <summary>
    /// RCA API URL for the service.
    /// </summary>
    public string RcaServiceApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Controls how the System.Text.Json.Serialization.JsonIgnoreAttribute ignores properties
    /// on serialization and deserialization.
    /// </summary>
    public JsonIgnoreCondition DefaultIgnoreCondition { get; set; } = JsonIgnoreCondition.WhenWritingNull;

    /// <summary>
    /// Defines whether JSON should pretty print which includes:
    /// indenting nested JSON tokens, adding new lines, and adding white space between property names and values.
    /// By default, the JSON is serialized without any extra white space.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this property is set after serialization or deserialization has occurred.
    /// </exception>
    public bool WriteIndented { get; set; } = false;

    /// <summary>
    /// Minimum log level for the application, used to filter log messages. Read from the Logging section of the configuration.
    /// </summary>
    public LogLevel MinLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Gets or sets the delay, in milliseconds, used for the delay in milliseconds that the publisher backgroud task
    /// will wait before checking for a new event.
    /// </summary>
    public int PollingDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether unsafe relaxed JSON escaping is enabled.
    /// </summary>
    /// <remarks>When enabled, this property allows JSON escaping rules to be relaxed, which may improve
    /// performance  but could introduce security risks if untrusted input is processed. Use with caution in scenarios 
    /// where input data may not be fully trusted.</remarks>
    public bool UnsafeRelaxedJsonEscaping { get; set; } = false;

    /// <summary>
    /// Gets or sets the HTTP timeout duration, in seconds, for network requests.
    /// </summary>
    /// <remarks>Setting this property to a lower value may result in faster failure for slow network
    /// requests,  while a higher value allows more time for requests to complete. Ensure the value is appropriate  for
    /// the expected network conditions.</remarks>
    public int HttpTimeout { get; set; } = 30;

    /// <summary>
    /// Gets or sets the circuit breaker settings.
    /// </summary>
    public CircuitBreakerSettings CircuitBreakerSettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the retry settings.
    /// </summary>
    public RetrySettings RetrySettings { get; set; } = new();

    /// <summary>
    /// Gets or sets the bulkhead settings that control the maximum concurrency and queue size for operations.
    /// </summary>
    public BulkheadSettings BulkheadSettings { get; set; } = new();
}
