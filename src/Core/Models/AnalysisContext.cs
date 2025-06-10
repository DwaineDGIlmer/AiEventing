namespace Core.Models
{
    /// <summary>
    /// Represents the context for an analysis operation, including configuration settings and environment details.
    /// </summary>
    /// <remarks>This class encapsulates the necessary information for performing an analysis, such as the
    /// processing region, selected language model, vector service endpoint, analysis policy, and environment. It is
    /// typically used to configure and manage analysis-related operations.</remarks>
    public class AnalysisContext
    {
        /// <summary>
        /// Gets or sets the region where processing operations are performed.
        /// </summary>
        public string ProcessingRegion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selection identifier for the Large Language Model (LLM) to be used.
        /// </summary>
        public string LlmSelection { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the endpoint URL for the vector service.
        /// </summary>
        public string VectorServiceEndpoint { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the analysis policy used to configure the behavior of the analysis process.
        /// </summary>
        public string AnalysisPolicy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the environment in which the application is running.
        /// </summary>
        public string Environment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of workflow names.
        /// </summary>
        public List<string>? Workflows { get; set; } = null;
    }
}
