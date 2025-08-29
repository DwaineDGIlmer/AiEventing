using Core.Contracts;
using Domain.Analysis;

namespace Core.Models;

/// <summary>
/// Root class representing a comprehensive incident summary, including executive, technical, remediation, and next action details.
/// </summary>
public class AnalysisSummary : IAnalysisSummary
{
    /// <summary>Technical summary with detailed technical reasons and references.</summary>
    public TechnicalSummary TechnicalSummary { get; set; } = new();

    /// <summary> Gets or sets the customer-specific context for the current operation. </summary>
    public KnownIssue KnownIssue { get; set; } = new();

    /// <summary>Gets or sets the high-level executive summary of the incident. </summary>
    public NextActions NextActions { get; set; } = new();
}
