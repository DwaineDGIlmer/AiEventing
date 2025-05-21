using Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json.Serialization;

namespace Core.Configuration;

/// <summary>
/// Settings used across the eventing service.
/// </summary>
public class AiEventSettings
{
    /// <summary>
    /// The name of the REST service client used for making requests to the AI service.
    /// </summary>
    public string FaultServiceClientName { get; set; } = nameof(FaultAnalysisService);

    /// <summary>
    /// The model to use for the AI service.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL of the API used for making requests to AI service.
    /// </summary>
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>
    /// API URL for the service.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

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
    public int HttpTimeout { get; set; } = 10;

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
