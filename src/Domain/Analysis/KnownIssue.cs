namespace Domain.Analysis;

/// <summary>
/// Represents an issue that has been identified and documented.
/// </summary>
/// <remarks>This class is used to encapsulate information about known issues, including whether the issue
/// is recognized and additional details describing the issue. It can be used to track and communicate issues in
/// applications or systems.</remarks>
public class KnownIssue
{
    /// <summary>Indicates if this is a known issue.</summary>
    public bool IsKnown { get; set; } = false;

    /// <summary>Details about the known issue.</summary>
    public string Details { get; set; } = string.Empty;

    /// <summary>Gets or sets the list of references related to the analysis, such as documentation or links.</summary>
    public IList<ExternalReference> References { get; set; } = [];
}
