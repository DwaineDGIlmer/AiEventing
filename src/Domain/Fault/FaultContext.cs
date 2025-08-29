namespace Domain.Fault;

/// <summary>
/// Represents the context for analyzing faults, including analysis details, exception information, and
/// customer-specific data.
/// </summary>
/// <remarks>This class aggregates multiple contexts to provide a comprehensive view of fault analysis. It
/// includes the analysis context, exception context, and customer context, which can be used to diagnose and
/// resolve issues effectively.</remarks>
public class FaultContext
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source identifier for the current context.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation timestamp of the entity.
    /// </summary>
    public string CreatedAt { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of workflow identifiers.
    /// </summary>
    public List<string>? Workflows { get; set; } = null;
}
