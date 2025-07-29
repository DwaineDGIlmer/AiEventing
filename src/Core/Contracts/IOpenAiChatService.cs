using Core.Configuration;
using OpenAI;

namespace Core.Contracts;

/// <summary>
/// Represents a client interface for interacting with OpenAI services.
/// </summary>
/// <remarks>This interface provides access to an underlying <see cref="OpenAIClient"/> instance,  which
/// can be used to perform operations supported by the OpenAI API.</remarks>
public interface IOpenAiChatService : IAiChatService
{
    /// <summary>
    /// Gets the instance of the <see cref="OpenAIClient"/> used to interact with the OpenAI API.
    /// </summary>
    public OpenAIClient Client { get; }

    /// <summary>
    /// Gets or sets the configuration settings for interacting with the OpenAI API.
    /// </summary>
    public OpenAiSettings Configuration { get; }
}