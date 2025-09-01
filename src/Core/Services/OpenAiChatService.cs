using Core.Configuration;
using Core.Contracts;
using Core.Extensions;
using Core.Helpers;
using Core.Serializers;
using OpenAI;
using OpenAI.Chat;

namespace Core.Services;

/// <summary>
/// Provides services for interacting with OpenAI's API, including generating embeddings and chat completions.
/// </summary>
/// <remarks>This class acts as a wrapper around the OpenAI client, enabling simplified access to OpenAI's
/// functionality. It supports generating numerical embeddings for text and creating chat completions based on
/// system and user messages. The service leverages caching to optimize performance and reduce redundant API
/// calls.</remarks>
sealed public class OpenAiChatService : IOpenAiChatService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private ICacheService CacheService { get; }

    private ILogger<OpenAiChatService> Logger { get; }

    /// <summary>
    /// Gets the expected dimension of the data or structure being processed.
    /// </summary>
    public ulong ExpectedDimension { get; }

    /// <summary>
    /// <inheritdoc cref="IOpenAiChatService.Client"/>
    /// </summary>
    public OpenAIClient Client { get; }

    /// <summary>
    /// <inheritdoc cref="IOpenAiChatService.Configuration"/>
    /// </summary>
    public OpenAiSettings Configuration { get; }

    /// <summary>
    /// Provides services for interacting with OpenAI's API, including generating chat completions.
    /// </summary>
    /// <remarks>This class acts as a wrapper around the OpenAI client, enabling simplified access to OpenAI's
    /// chat completion functionality. It requires an instance of <see cref="OpenAIClient"/> and configuration settings
    /// via <see cref="OpenAiSettings"/>.</remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="OpenAiChatService"/> class.
    /// </remarks>
    /// <param name="client">The <see cref="OpenAIClient"/> instance used to interact with the OpenAI API. Cannot be <see
    /// langword="null"/>.</param>
    /// <param name="options">The configuration settings for the OpenAI service. Cannot be <see langword="null"/>.</param>
    /// <param name="cacheService">The <see cref="ICacheService"/> instance used for caching results. Cannot be <see langword="null"/>.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public OpenAiChatService(
        OpenAIClient client,
        IOptions<OpenAiSettings> options,
        ICacheService cacheService,
        ILogger<OpenAiChatService> logger)
    {
        var openAiSettings = options.IsNullThrow().Value ?? throw new ArgumentNullException(nameof(options), "OpenAiSettings cannot be null.");

        CacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService), "CacheService cannot be null.");
        Client = client ?? throw new ArgumentNullException(nameof(client), "OpenAIClient cannot be null.");
        Configuration = openAiSettings ?? throw new ArgumentNullException(nameof(options), "OpenAiSettings cannot be null.");
        Logger = logger ?? throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
    }

    /// <inheritdoc/>
    public async Task<T> GetChatCompletion<T>(string systemMessage, string userMessage) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(systemMessage) || string.IsNullOrWhiteSpace(userMessage))
        {
            Logger.LogError("OpenAi, no messages provided for chat completion.");
            return new();
        }

        // Generate a unique cache key based on the system and user messages to avoid redundant API calls.
        var cacheKey = CachingHelper.GenCacheKey(nameof(OpenAiChatService), systemMessage, userMessage.GenHashString());
        var cacheItem = await CacheService.TryGetAsync<T>(cacheKey);
        if (cacheItem is not null)
        {
            Logger.LogInformation("Chat result served from cache.");
            return cacheItem;
        }
        // Create a chat client and prepare the messages for the chat completion request.
        var chatClient = Client.GetChatClient(Configuration.Model);
        List<ChatMessage> messages =
        [
            new SystemChatMessage(systemMessage),
            new AssistantChatMessage(userMessage)
        ];

        // Call the OpenAI API to complete the chat with the provided messages.
        var completion = await chatClient.CompleteChatAsync(messages);
        var content = completion.Value.Content.FirstOrDefault();
        if (content is not null && content.Text is not null)
        {
            var responseDoc = JsonConvertService.Instance.Deserialize<T>(JsonHelpers.ExtractJson(content.Text), _options);
            await CacheService.CreateEntryAsync(cacheKey, responseDoc);
            return responseDoc as T ?? throw new Exception("Chat completion content is not of the expected type.");
        }
        throw new Exception("Chat completion content is null or empty.");
    }
}
