using Core.Models;

namespace Core.Contracts;

/// <summary>
/// Represents the context for fault analysis operations, providing essential information such as identifiers,
/// timestamps, and associated contexts.
/// </summary>
/// <remarks>This interface is used to encapsulate the necessary data for analyzing faults within a
/// system. It includes identifiers for the entity and source,  timestamps for creation, and collections of related
/// workflows. Additionally, it provides context information for exceptions and customer-specific data.</remarks>
public interface IFaultAnalysisContext
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the source identifier for the current operation.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp of the entity.
    /// </summary>
    public string CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of workflow names.
    /// </summary>
    public List<string>? Workflows { get; set; }

    /// <summary>
    /// Gets or sets the context information associated with an exception.
    /// </summary>
    public ExceptionContext ExceptionContext { get; set; }

    /// <summary>
    /// Gets or sets the customer context associated with the current operation.
    /// </summary>
    public CustomerContext CustomerContext { get; set; }
}
