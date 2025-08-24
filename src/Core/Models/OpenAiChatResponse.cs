namespace Core.Models;

/// <summary>
/// Represents the response from a chat completion API, containing metadata, choices, and usage statistics.
/// </summary>
public class OpenAiChatResponse
{
    /// <summary>
    /// Gets or sets the unique identifier for the chat completion response.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type or category of the object represented by this instance.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp indicating when the object was created.
    /// </summary>
    [JsonPropertyName("created")]
    public long Created { get; set; }

    /// <summary>
    /// Gets or sets the name of the model used for processing or operations.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of choices returned by the operation.
    /// </summary>
    [JsonPropertyName("choices")]
    public IList<CompletionChoice> Choices { get; set; } = [];

    /// <summary>
    /// Gets or sets the usage details associated with the current operation.
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage Usage { get; set; } = new Usage();
}

/// <summary>
/// Represents a single choice in a response, including its index, associated message, and the reason the choice was
/// completed.
/// </summary>
/// <remarks>This class is typically used to encapsulate the details of a choice in a multi-choice response
/// scenario.</remarks>
public class CompletionChoice
{
    /// <summary>
    /// Gets or sets the index value associated with the object.
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the current operation.
    /// </summary>
    [JsonPropertyName("message")]
    public OpenAiMessage Message { get; set; } = new();

    /// <summary>
    /// Gets or sets the reason why the operation or process was completed.
    /// </summary>
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; } = string.Empty;
}

/// <summary>
/// Represents the token usage details for a request, including prompt, completion, and total tokens.
/// </summary>
/// <remarks>This class is typically used to track the number of tokens consumed during a request,  such as in
/// natural language processing or API interactions where token limits apply.</remarks>
public class Usage
{
    /// <summary>
    /// Gets or sets the number of tokens consumed by the prompt in a request.
    /// </summary>
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens used to generate the completion response.
    /// </summary>
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    /// <summary>
    /// Gets or sets the total number of tokens processed in the operation.
    /// </summary>
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    /// <summary>
    /// Gets or sets the optional details of the tokens used in the prompt.
    /// </summary>
    [JsonPropertyName("prompt_tokens_details")]
    public IDictionary<string, int>? PromptTokensDetails { get; set; }

    /// <summary>
    /// Gets or sets the optional details of the tokens generated during the completion process.
    /// </summary>
    [JsonPropertyName("completion_tokens_details")]
    public IDictionary<string, int>? CompletionTokensDetails { get; set; }
}

/// <summary>
/// Represents the details of a token, including its role, content, and token count.
/// </summary>
/// <remarks>This class is typically used to encapsulate information about a token in a structured format. It
/// includes the role associated with the token, the content of the token, and the number of tokens.</remarks>
public class TokenDetail
{
    /// <summary>
    /// Gets or sets the role associated with the entity.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content associated with the object.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of tokens associated with the current operation or entity.
    /// </summary>
    [JsonPropertyName("tokens")]
    public int Tokens { get; set; }
}
