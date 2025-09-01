using Core.Constants;
using Core.Models;

namespace Core.Configuration;

/// <summary>
/// Represents the configuration settings required to interact with the OpenAI service.
/// </summary>
/// <remarks>This class encapsulates the necessary parameters for authenticating and communicating with
/// the OpenAI API, including the API key, endpoint URL, and model identifier. These settings are typically used to
/// configure API clients or services that integrate with OpenAI.</remarks>
public sealed class SerpApiSettings : RestApiSettings
{
    /// <summary>
    /// Gets or sets the cache expiration time in minutes.
    /// </summary>
    public int CacheExpirationInMinutes { get; set; } = 1440;

    /// <summary>
    /// Gets or sets the directory path where company profile files are stored.
    /// </summary>
    public string FileCompanyProfileDirectory { get; set; } = Defaults.FileCompanyProfileDirectory;

    /// <summary>
    /// Gets or sets the directory path where job profile files are stored.
    /// </summary>
    public string FileJobProfileDirectory { get; set; } = Defaults.FileJobProfileDirectory;

    /// <summary>
    /// Gets or sets the query string to be executed.
    /// </summary>
    public string Query { get; set; } = Defaults.SerpApiQuery;

    /// <summary>
    /// Gets or sets the location identifier.
    /// </summary>
    public string Location { get; set; } = Defaults.SerpApiLocation;

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    override public string? ApiKey { get; set; } = null;

    /// <summary>
    /// Gets or sets the URL of the API endpoint for the AI service.
    /// </summary>
    override public string BaseAddress { get; set; } = Defaults.SerpApiBaseAddress;

    /// <summary>
    /// Gets or sets the search endpoint URL used for querying the search service.
    /// </summary>
    override public string Endpoint { get; set; } = Defaults.SearchEndpoint;

    /// <summary>
    /// Gets or sets the name of the OpenAI client.
    /// </summary>
    override public string HttpClientName { get; set; } = Defaults.SerpApiClientName;
}
