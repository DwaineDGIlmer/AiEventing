namespace Domain.Incident;

/// <summary>
/// Represents a high-level summary of an incident, including key details such as the application involved,  failing
/// components, severity, and causes. This class provides a structured overview of the incident  for reporting and
/// analysis purposes.
/// </summary>
/// <remarks>The <see cref="ExecutiveSummary"/> class aggregates information about an incident, including
/// internal  and external issues, environmental failures, and detailed cause analysis. It is designed to provide  a
/// concise yet comprehensive summary for executive-level review or incident management.</remarks>
public class ExecutiveSummary
{
    /// <summary>Unique identifier for the incident.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>High-level summary of the incident.</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>Name of the application involved.</summary>
    public string Application { get; set; } = string.Empty;

    /// <summary>Name of the failing component.</summary>
    public string FailingComponent { get; set; } = string.Empty;

    /// <summary>Severity of the incident.</summary>
    public string Severity { get; set; } = string.Empty;

    /// <summary>Brief explanation of the cause.</summary>
    public string HighLevelCause { get; set; } = string.Empty;

    /// <summary>List of internal issues.</summary>
    public List<InternalIssue> InternalIssues { get; set; } = [];

    /// <summary>List of external issues.</summary>
    public List<ExternalIssue> ExternalIssues { get; set; } = [];

    /// <summary>List of environmental failures.</summary>
    public List<EnvironmentalFailure> EnvironmentalFailures { get; set; } = [];

    /// <summary>Cause details (who/what and preventability).</summary>
    public Cause Cause { get; set; } = new Cause();
}