namespace Core.Contracts;

/// <summary>
/// Defines a service for generating AI-driven chat completions based on user input and system context.
/// </summary>
/// <remarks>This interface provides a method to interact with an AI model to generate chat responses.
/// Implementations of this interface are expected to handle the communication with the underlying AI system.</remarks>
public interface IAiChatService
{
    /// <summary>
    /// Sends a system message and a user message to generate a chat completion response.
    /// </summary>
    /// <remarks>This method is designed to interact with a chat model, where the <paramref
    /// name="systemMessage"/> sets the context or behavior and the <paramref name="userMessage"/> provides the
    /// user's input. The response type <typeparamref name="T"/> should match the expected format of the chat
    /// model's output.</remarks>
    /// <typeparam name="T">The type of the response object returned by the chat completion.</typeparam>
    /// <param name="systemMessage">The system message that provides context or instructions for the chat model. Cannot be null or empty.</param>
    /// <param name="userMessage">The user message that represents the input or query for the chat model. Cannot be null or empty.</param>
    /// <returns>A task representing the asynchronous operation. The result contains the chat completion response of type
    /// <typeparamref name="T"/>.</returns>
    public Task<T> GetChatCompletion<T>(string systemMessage, string userMessage) where T : class, new();
}
