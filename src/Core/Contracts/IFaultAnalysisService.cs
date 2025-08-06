namespace Core.Contracts;

/// <summary>
/// Defines methods for analyzing faults and generating responses based on fault descriptions or exceptions.
/// </summary>
/// <remarks>This interface provides functionality to analyze faults using either a textual description or
/// an exception object,  along with a collection of related messages, and returns a structured response.</remarks>
public interface IFaultAnalysisService
{
    /// <summary>
    /// Analyzes the specified fault log event to determine its validity and severity.
    /// </summary>
    /// <param name="fault">The log event representing the fault to be analyzed. Cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the fault is determined to be valid and actionable; otherwise, <see
    /// langword="false"/>.</returns>
    Task<bool> AnalyzeFaultAsync(ILogEvent fault);
}
