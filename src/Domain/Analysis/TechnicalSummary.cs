namespace Domain.Analysis;

/// <summary>
/// Represents a summary of technical details related to an incident, including the root cause, stack trace
/// references, external documentation, known issues, and exception tags.
/// </summary>
/// <remarks>This class is designed to provide a comprehensive overview of the technical aspects of an
/// incident, enabling detailed analysis and troubleshooting. It includes information such as the technical reason
/// for the incident, references to stack trace locations, links to external documentation, known issues, and tags
/// associated with exceptions.</remarks>
public class TechnicalSummary
{
    /// <summary>Detailed technical reason for the incident.</summary>
    public string TechnicalReason { get; set; } = string.Empty;

    /// <summary>External documentation or references.</summary>
    public List<ExternalReference> ExternalReferences { get; set; } = [];
}
