namespace Domain.Incident;

/// <summary>
/// Represents an internal issue within an incident, including a description and whether it was preventable.
/// </summary>
public class InternalIssue
{
    /// <summary>SuggestedFix of the internal issue.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Indicates if the issue was preventable.</summary>
    public bool Preventable { get; set; } = false;
}
