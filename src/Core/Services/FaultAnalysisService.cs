using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using OpenAI.Chat;
using System.Runtime.CompilerServices;

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
/// <param name="openAiService">The OpenAI service.</param>
/// <param name="aiEventsettings">Settings used to configure the service.</param>
public sealed class FaultAnalysisService(
    IOpenAiChatService openAiService,
    IOptions<AiEventSettings> aiEventsettings) : IFaultAnalysisService
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="FaultAnalysisService"/> class.
    /// </summary>
    internal IOpenAiChatService ChatService { get; set; } = openAiService.IsNullThrow();

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
    /// Gets a value indicating whether the RCA (Root Cause Analysis) service is enabled.
    /// </summary>           
    internal bool FaultServiceEnabled => aiEventsettings.IsNullThrow().Value.FaultServiceEnabled;

    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/> used for serialization and deserialization.
    /// </summary>
    internal JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Analyzes a fault by sending it to a remote API for processing.
    /// </summary>
    /// <remarks>This method sends the provided fault event as a serialized JSON payload to a remote
    /// API endpoint. The API response is validated and deserialized to determine the result.</remarks>
    /// <param name="fault">The fault event to be analyzed. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the fault was successfully analyzed; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> AnalyzeFaultAsync(ILogEvent fault)
    {
        if (!FaultServiceEnabled)
        {
            Logger.LogWarning("RCA analysis functionality is disabled. Skipping fault analysis.");
            return false;
        }

        fault.IsNullThrow();
        _semaphore.Wait();
        try
        {
            var message = $"Exception below:\r\n{fault.Exception}\r\n";
            if (fault.InnerExceptions?.Count > 0)
            {
                message += "Inner Exceptions:\r\n";
                foreach (var inner in fault.InnerExceptions)
                {
                    message += $"{inner}\r\n";
                }
            }

            List<ChatMessage> messages =
            [
                new SystemChatMessage("Given the following input exception details, provide a clear and concise explanation of what happened and why it happened. If there are any known issues related to this exception message or stack frame (including those found in the provided knowledge base context) mention them. Suggest actionable steps the user can take to resolve or fix the issue, and provide references when possible. The response should be structure in json format like below:\r\n{    \"error\": { \"type\": \"System.InvalidOperationException\", \"message\": \"Operation is not valid.\", \"cause\": \"This exception occurred because an operation that was performed is not considered valid in the current context.\",\r\n        \"stackTrace\": \"The exception originated in the Generator.test method at line 54 of the file test.cs in the WebApplication project.\",\r\n        \"possibleCauses\": [],\r\n        \"suggestedActions\": [\r\n            \"Review the code at line 54 in test.cs to identify the specific operation being performed.\",\r\n            \"Verify that all inputs and conditions necessary for the operation at line 54 are valid and properly handled.\", \"Check if there are any external factors impacting the validity of the operation.\"] }}."),
                new UserChatMessage($"Exception below:\r\n{fault.Exception}\r\n")
            ];

            var chatClient = ChatService.Client.GetChatClient(ChatService.Configuration.Model);
            var response = await chatClient.CompleteChatAsync(messages);
            response.IsNullThrow();

            if (response is null || response.Value is null || response.Value.Content.Count == 0)
            {
                Logger.LogWarning("No choices returned from the AI service for fault analysis.");
                return false;
            }
            fault.Body = response.Value.Content.FirstOrDefault()?.Text?.Trim() ?? string.Empty;
        }
        finally
        {
            _semaphore.Release();
        }
        return true;
    }
}
