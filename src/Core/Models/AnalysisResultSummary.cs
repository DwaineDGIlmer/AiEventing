using Core.Contracts;

namespace Core.Models;

/// <summary>
/// Represents a summary of the analysis result, including details such as the unique identifier, customer information,
/// source, fault type, and any processing errors encountered.
/// </summary>
public class AnalysisResultSummary : AnalysisSummary
{
    /// <summary>
    /// Unique identifier for the analysis result summary.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the customer.
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source of the analysis, which can be used to identify where the data originated from.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of fault associated with the current operation or entity.
    /// </summary>
    public string FaultType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when this ingestion request was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets any errors encountered during the processing of the analysis, which can include issues such as data inconsistencies or processing failures.
    /// </summary>
    public IList<Error>? ProcessingErrors { get; set; } = null;

    /// <summary>
    /// The default constuctor.
    /// </summary>
    public AnalysisResultSummary() { }

    /// <summary>
    /// Takes a IngestionRequest and adds the relevant sections to the summary instance.
    /// </summary>
    /// <param name="ingestionRequest">The IngestionRequest to copy.</param>
    public AnalysisResultSummary(IIngestionRequest ingestionRequest)
    {
        Id = ingestionRequest.RequestId;
        CustomerId = ingestionRequest.FaultContext.CustomerContext.Id;
        Source = ingestionRequest.Source;
        FaultType = ingestionRequest.FaultContext.ExceptionContext.ExceptionType;
        Timestamp = ingestionRequest.Timestamp;
        ProcessingErrors = [];
    }
}
