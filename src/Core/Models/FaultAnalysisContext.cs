namespace Core.Models
{
    /// <summary>
    /// Represents the context for analyzing faults, including analysis details, exception information, and
    /// customer-specific data.
    /// </summary>
    /// <remarks>This class aggregates multiple contexts to provide a comprehensive view of fault analysis. It
    /// includes the analysis context, exception context, and customer context, which can be used to diagnose and
    /// resolve issues effectively.</remarks>
    public class FaultAnalysisContext
    {
        /// <summary>Unique identifier for the incident.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Source or source provider of this fault report.</summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>Date and time when the incident occurred.</summary>
        public string CreatedAt { get; set; } = string.Empty;

        /// <summary>Gets or sets the context used for analysis operations.</summary>
        public AnalysisContext AnalysisContext { get; set; } = new();

        /// <summary>Gets or sets the context information related to an exception.</summary>
        public ExceptionContext ExceptionContext { get; set; } = new();

        /// <summary>Gets or sets the customer context associated with the current operation.</summary>
        public CustomerContext CustomerContext { get; set; } = new();
    }
}
