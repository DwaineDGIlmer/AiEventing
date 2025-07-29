using Core.Models;

namespace Core.Contracts
{
    /// <summary>
    /// Represents a summary of an analysis, including high-level details, key metrics, technical insights, remediation
    /// steps, and next actions.
    /// </summary>
    /// <remarks>This interface is designed to provide a comprehensive overview of an incident or analysis. It
    /// includes both executive-level summaries and detailed technical information, as well as actionable steps for
    /// remediation and follow-up.</remarks>
    public interface IAnalysisSummary
    {
        /// <summary>Technical summary with detailed technical reasons and references.</summary>
        public TechnicalSummary TechnicalSummary { get; set; }

        /// <summary>
        /// Gets or sets if this analysis is related to a known issue, including details about the issue.
        /// </summary>
        public KnownIssue KnownIssue { get; set; }

        /// <summary>
        /// Gets or sets next actions to be taken, including technical contacts and follow-up steps.
        /// </summary>
        public NextActions NextActions { get; set; }
    }
}
