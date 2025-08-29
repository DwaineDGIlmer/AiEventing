namespace Domain.Analysis;

/// <summary>
/// Represents an external reference, including its type, URL, and description.
/// </summary>
/// <remarks>This class is typically used to store metadata about external resources, such as links to
/// vendor documentation or other supporting materials. Each instance contains information about the type of
/// reference, its URL, and an optional description providing additional context.</remarks>
public class ExternalReference
{
    /// <summary>Type of external reference (e.g., Vendor Documentation).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>URL to the external reference.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>SuggestedFix of the reference.</summary>
    public string Description { get; set; } = string.Empty;
}
