using Core.Configuration;
using Core.Constants;
using Core.Contracts;
using Core.Extensions;
using Core.Models;
using Core.Serializers;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("Logger.UnitTets, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
[assembly: InternalsVisibleTo("Core.UnitTests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c599f69c3dd3ec398aa236557324d13db0f01fe1619e95bd66ab4fbd53143b2e57470a9c156080f2e3b088da0a7d40ce549ed7d803bc7cfc904077dce8ea5262c4afc77594841fb916db84485db81dfa6ba6cba1d449c0cb8c6aafd42245221dc310fae03f9b18c258c7939cd293f01a9b1ab9c433a53b278022f02a46958797")]
namespace Core.Services;

/// <summary>
/// Provides functionality for analyzing faults using AI services.
/// </summary>
/// <remarks>The <see cref="FaultAnalysisService"/> class integrates with external AI services to analyze
/// fault-related data. It supports sending fault descriptions and events to remote APIs for processing and returns
/// the analysis results. This service requires proper configuration of API keys, endpoints, and model
/// identifiers.</remarks>
/// <param name="httpClientFactory">Http client factory for building http clients.</param>
/// <param name="settings">Settings used to configure the service.</param>
public class FaultAnalysisService(IHttpClientFactory httpClientFactory, AiEventSettings settings) : IFaultAnalysisService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FaultAnalysisService"/> class.
    /// </summary>
    internal IHttpClientFactory HttpFactory { get; set; } = httpClientFactory.IsNullThrow();

    /// <summary>
    /// Gets or sets the logger instance used for logging diagnostic and operational information related to the
    /// fault analysis service.
    /// </summary>
    internal ILogger Logger
    {
        get
        {
            if (_logger.IsNull())
            {
                // Fixing CS1503 by providing a proper Action<ILoggingBuilder> delegate instead of a string.
                return _logger = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                }).CreateLogger<IFaultAnalysisService>();
            }
            return _logger;
        }
        set
        {
            if (value.IsNull())
            {
                throw new ArgumentNullException(nameof(value), "Logger cannot be null.");
            }
            _logger = value;
        }
    }
    private ILogger? _logger;

    /// <summary>
    /// Gets a value indicating whether OpenAI functionality is enabled.
    /// </summary>
    internal bool OpenAiEnabled => settings.IsNullThrow().OpenAiEnabled;

    /// <summary>
    /// Gets a value indicating whether the RCA (Root Cause Analysis) service is enabled.
    /// </summary>           
    internal bool RcaServiceEnabled => settings.IsNullThrow().RcaServiceEnabled;

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    internal string OpenAiClient { get; set; } = settings.IsNullThrow().OpenAiClient.IsNullThrow();

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    internal string RcaServiceClient { get; set; } = settings.IsNullThrow().RcaServiceClient.IsNullThrow();

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    internal string RcaServiceApiKey { get; set; } = settings.IsNullThrow().RcaServiceApiKey.IsNullThrow();

    /// <summary>
    /// Gets or sets the URL of the API endpoint for the AI service.
    /// </summary>
    internal string RcaServiceApiPath { get; set; } = new Uri(settings.IsNullThrow().RcaServiceUrl.IsNullThrow()).AbsolutePath;

    /// <summary>
    /// Gets or sets the API key used for authentication with the AI service.
    /// </summary>
    internal string OpenAiApiKey { get; set; } = settings.IsNullThrow().OpenAiApiKey.IsNullThrow();

    /// <summary>
    /// Gets or sets the URL of the API endpoint for the AI service.
    /// </summary>
    internal string OpenAiApiPath { get; set; } = new Uri(settings.IsNullThrow().OpenAiApiUrl.IsNullThrow()).AbsolutePath;

    /// <summary>
    /// Gets or sets the model identifier used for the AI service.
    /// </summary>
    internal string OpenAiModel { get; set; } = settings.IsNullThrow().OpenAiModel.IsNullThrow();

    /// <summary>
    /// Gets or sets the collection of messages associated with the current fault analysis context.
    /// </summary>
    internal IList<OpenAiMessage> Messages { get; set; } = [];

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    internal JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Fault analysis method that sends a fault description to the AI service and returns the analysis result.
    /// </summary>
    /// <param name="messages">A collection of messages providing context for the analysis. Cannot be <see langword="null"/>.</param>
    /// <returns>Task results of the request.</returns>
    public async Task<OpenAiChatResponse> AnalyzeFaultAsync(IList<OpenAiMessage> messages)
    {
        if (!OpenAiEnabled)
        {
            Logger.LogWarning("OpenAI functionality is disabled. Skipping fault analysis.");
            return new OpenAiChatResponse();
        }

        messages.IsNullThrow();
        var request = new OpenAiChatRequest
        {
            Model = OpenAiModel,
            Messages = (List<OpenAiMessage>)messages
        };

        // Will fail here if instance is null.
        var requestBody = JsonConvertService.Instance!.Serialize(request, Options);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, OpenAiApiPath)
        {
            Headers =
            {
                { "Authorization", $"Bearer {OpenAiApiKey}" }
            },
            Content = new StringContent(requestBody, Encoding.UTF8, Defaults.JsonMimeType)
        };

        var response = await HttpFactory.CreateClient(OpenAiClient).SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError("Failed to analyze fault. Status code: {StatusCode}", response.StatusCode);
        }
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(responseBody))
        {
            Logger.LogWarning("Received empty response body from AI service.");
        }
        responseBody.IsNullThrow();
        return JsonConvertService.Instance.Deserialize<OpenAiChatResponse>(responseBody);
    }

    /// <summary>
    /// Analyzes a fault by sending it to a remote API for processing.
    /// </summary>
    /// <remarks>This method sends the provided fault event as a serialized JSON payload to a remote
    /// API endpoint. The API response is validated and deserialized to determine the result.</remarks>
    /// <param name="fault">The fault event to be analyzed. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the fault was successfully analyzed; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> AnalyzeFaultAsync(ILogEvent fault)
    {
        if (!RcaServiceEnabled)
        {
            Logger.LogWarning("RCA analysis functionality is disabled. Skipping fault analysis.");
            return false;
        }

        fault.IsNullThrow();

        // Will fail here if instance is null.
        var requestBody = JsonConvertService.Instance!.Serialize(fault, Options);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, OpenAiApiPath)
        {
            Headers =
            {
                { "Authorization", $"Bearer {RcaServiceApiKey}" }
            },
            Content = new StringContent(requestBody, Encoding.UTF8, Defaults.JsonMimeType)
        };

        var response = await HttpFactory.CreateClient(RcaServiceClient).SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError("Failed to analyze fault. Status code: {StatusCode}", response.StatusCode);
        }
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(responseBody))
        {
            Logger.LogWarning("Received empty response body from AI service.");
        }
        responseBody.IsNullThrow();
        return true;
    }
}
