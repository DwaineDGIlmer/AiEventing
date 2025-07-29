using Application.Models;
using Core.Helpers;
using System.Text.Json.Serialization;

namespace Core.Models
{

    /// <summary>
    /// Root class representing a comprehensive incident summary, including executive, technical, remediation, and next action details.
    /// </summary>
    public class ChatAnalysisSummary
    {
        /// <summary>Technical summary with detailed technical reasons and references.</summary>
        [JsonPropertyName("technicalSummary")]
        public string TechnicalSummary { get; set; } = string.Empty;

        /// <summary> Gets or sets the customer-specific context for the current operation. </summary>
        [JsonPropertyName("knownIssues")]
        public string KnownIssue { get; set; } = string.Empty;

        /// <summary>Gets or sets the high-level executive summary of the incident. </summary>
        [JsonPropertyName("nextActions")]
        public string NextActions { get; set; } = string.Empty;

        /// <summary> Gets or sets the remediation summary, including steps taken to resolve the incident. </summary>
        [JsonPropertyName("confidenceScore")]
        public decimal ConfidenceScore { get; set; }

        /// <summary>Gets or sets the list of references related to the analysis, such as documentation or links.</summary>
        [JsonPropertyName("references")]
        public IList<string> References { get; set; } = [];

        /// <summary>
        /// Gets the analysis result summary based on the current instance properties.
        /// </summary>
        public AnalysisResultSummary AnalysisSummaryResult
        {
            get
            {
                return new AnalysisResultSummary
                {
                    TechnicalSummary = new TechnicalSummary
                    {
                        TechnicalReason = TechnicalSummary,
                        ExternalReferences = JsonHelpers.ParseUrls(References),
                    },
                    KnownIssue = new KnownIssue
                    {
                        IsKnown = !string.IsNullOrEmpty(KnownIssue),
                        Details = KnownIssue,
                        References = JsonHelpers.ParseUrls(References)
                    },
                    NextActions = new NextActions { Description = NextActions },
                };
            }
        }

        /// <summary>
        /// Maps the current instance to an <see cref="AnalysisSummary"/>.
        /// </summary>
        /// <returns></returns>
        public AnalysisSummary AnalysisSummary
        {
            get
            {
                return new AnalysisSummary()
                {
                    TechnicalSummary = new TechnicalSummary
                    {
                        TechnicalReason = TechnicalSummary,
                        ExternalReferences = JsonHelpers.ParseUrls(References),
                    },
                    KnownIssue = new KnownIssue
                    {
                        IsKnown = !string.IsNullOrEmpty(KnownIssue),
                        Details = KnownIssue,
                        References = JsonHelpers.ParseUrls(References)
                    },
                    NextActions = new NextActions { Description = NextActions },
                };
            }
        }
    }
}
