namespace Core.Models;

/// <summary>
/// Represents the configuration settings required to interact with the OpenAI service.
/// </summary>
/// <remarks>This class encapsulates the necessary parameters for authenticating and communicating with
/// the OpenAI API, including the API key, endpoint URL, and model identifier. These settings are typically used to
/// configure API clients or services that integrate with OpenAI.</remarks>
public abstract class RestApiSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool IsCachingEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    public abstract string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the URL of the API endpoint for the AI service.
    /// </summary>
    public abstract string BaseAddress { get; set; }

    /// <summary>
    /// Gets or sets the endpoint URL for the service.
    /// </summary>
    public abstract string Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the name of the OpenAI client.
    /// </summary>
    public abstract string HttpClientName { get; set; }
}
