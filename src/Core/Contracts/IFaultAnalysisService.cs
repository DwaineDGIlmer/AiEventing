using Core.Models;

namespace Core.Contracts;

/// <summary>
/// Defines methods for analyzing faults and generating responses based on fault descriptions or exceptions.
/// </summary>
/// <remarks>This interface provides functionality to analyze faults using either a textual description or
/// an exception object,  along with a collection of related messages, and returns a structured response.</remarks>
public interface IFaultAnalysisService
{
    /// <summary>
    /// Analyzes the provided fault description and generates a response based on the given conversation context.
    /// </summary>
    /// <remarks>This method uses the provided fault description and conversation context to generate
    /// a response. The quality and relevance of the analysis depend on the completeness and clarity of the input
    /// parameters.</remarks>
    /// <param name="messages">A collection of messages representing the conversation context. This parameter cannot be null and must
    /// contain at least one message.</param>
    /// 
    /// <returns>A <see cref="OpenAiChatResponse"/> object containing the analysis result and any relevant insights.</returns>
    Task<OpenAiChatResponse> AnalyzeFaultAsync(IList<OpenAiMessage> messages);

    /// <summary>
    /// Analyzes the specified fault log event to determine its validity and severity.
    /// </summary>
    /// <param name="fault">The log event representing the fault to be analyzed. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the fault is determined to be valid and actionable; otherwise, <see
    /// langword="false"/>.</returns>
    Task<bool> AnalyzeFaultAsync(ILogEvent fault);
}
