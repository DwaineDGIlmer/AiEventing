namespace Domain.Incident;

/// <summary>
/// Represents an external issue related to an incident, including a description, type, and details.
/// </summary>
public class ExternalIssue
{
    /// <summary>Description of the external issue.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Type of external issue (e.g., Vendor).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Details of the vendor or external issue.</summary>
    public string Details { get; set; } = string.Empty;
}
