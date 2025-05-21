using System.Text.Json.Serialization;

namespace Core.Models;

/// <summary>
/// Represents a request to the ChatGPT API, containing the model to use and the list of messages for the conversation.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// Gets or sets the identifier of the model to be used for the ChatGPT request.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of messages to be sent in the ChatGPT request.
    /// Each <see cref="Message"/> represents a single message in the conversation history.
    /// </summary>
    [JsonPropertyName("messages")]
    public IList<Message> Messages { get; set; } = [];
}

/// <summary>
/// Represents a chat message, including the sender's role and the message content.
/// </summary>
/// <remarks>This class is typically used to model messages exchanged in a chat system, where each message
/// includes the sender's role (e.g., "user", "assistant", or "system") and the message text.</remarks>
public class Message
{
    /// <summary>
    /// Gets or sets the role of the message sender in the chat (e.g., "user", "assistant", or "system").
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
