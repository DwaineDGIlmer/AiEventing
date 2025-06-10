namespace Core.Models;

/// <summary>
/// Represents external context metadata to enrich LLM-based root cause analysis.
/// </summary>
public class ModelContext
{
    /// <summary>
    /// Logical environment (e.g., dev, test, staging, prod).
    /// </summary>
    public string Environment { get; set; } = string.Empty;

    /// <summary>
    /// Identifier for the deployment version or release tag.
    /// </summary>
    public string DeploymentId { get; set; } = string.Empty;

    /// <summary>
    /// Git SHA of the commit that was deployed.
    /// </summary>
    public string GitSha { get; set; } = string.Empty;

    /// <summary>
    /// Arbitrary custom metadata for the LLM to consume (feature flags, org info, etc).
    /// </summary>
    public Dictionary<string, string> CustomTags { get; set; } = new();
}
