using Core.Constants;
using Core.Models;

namespace Core.Configuration;

/// <summary>
/// Represents the configuration settings required to interact with the OpenAI service.
/// </summary>
/// <remarks>This class encapsulates the necessary parameters for authenticating and communicating with
/// the OpenAI API, including the API key, endpoint URL, and model identifier. These settings are typically used to
/// configure API clients or services that integrate with OpenAI.</remarks>
public class OpenAiSettings : RestApiSettings
{
    /// <summary>
    /// Gets or sets if the cache should be deleted on start or not.
    /// </summary>
    public bool ClearCache { get; set; } = false;

    /// <summary>
    /// Gets or sets the model identifier used for the AI service.
    /// </summary>
    public string Model { get; set; } = Defaults.OpenAiModel;

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    override public string? ApiKey { get; set; } = null;

    /// <summary>
    /// Gets or sets the URL of the API endpoint for the AI service.
    /// </summary>
    override public string BaseAddress { get; set; } = Defaults.OpenAiABaseAddress;

    /// <summary>
    /// Gets or sets the endpoint URL for the OpenAI service.
    /// </summary>
    override public string Endpoint { get; set; } = Defaults.OpenAiEndpoint;

    /// <summary>
    /// Gets or sets the name of the OpenAI client.
    /// </summary>
    override public string HttpClientName { get; set; } = Defaults.OpenAiClientName;
}
