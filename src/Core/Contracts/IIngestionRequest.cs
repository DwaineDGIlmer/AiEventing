namespace Core.Contracts;

/// <summary>
/// Represents an ingestion request containing metadata and context information for processing data from a specified
/// source.
/// </summary>
/// <remarks>This interface defines the structure for an ingestion request, including properties for
/// identifying the request, tracking its creation time, and associating it with a fault analysis context.
/// Implementations of this interface are expected to provide the necessary details for data ingestion
/// workflows.</remarks>
public interface IIngestionRequest
{
    /// <summary>
    /// Gets or sets the FaultAnalysisContext associated with this ingestion request.
    /// </summary>
    public IFaultAnalysisContext FaultContext { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for this ingestion request.
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this ingestion request was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the source of the data being ingested (e.g., file path, system name).
    /// </summary>
    public string Source { get; set; }
}
