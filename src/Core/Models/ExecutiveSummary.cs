namespace Core.Models;

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

/// <summary>
/// Represents an external issue related to an incident, including a description, type, and details.
/// </summary>
public class ExternalIssue
{
    /// <summary>SuggestedFix of the external issue.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Type of external issue (e.g., Vendor).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Details of the vendor or external issue.</summary>
    public string Details { get; set; } = string.Empty;
}

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

/// <summary>
/// Represents an environmental failure, such as a power outage or other external disruption.
/// </summary>
/// <remarks>This class provides information about the type and description of an environmental failure. It can be
/// used to log or track issues caused by external factors.</remarks>
public class EnvironmentalFailure
{
    /// <summary>Type of environmental failure (e.g., Power Outage).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>SuggestedFix of the environmental failure.</summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents the cause of an incident, including details about the responsible entity and whether the incident was
/// preventable.
/// </summary>
/// <remarks>This class provides information about the origin of an incident, such as the entity or factor
/// responsible for it,  and whether the incident could have been avoided. It can be used in scenarios where incident
/// analysis or reporting is required.</remarks>
public class Cause
{
    /// <summary>Who or what caused the incident.</summary>
    public string WhoOrWhat { get; set; } = string.Empty;

    /// <summary>Indicates if the cause was preventable.</summary>
    public bool Preventable { get; set; } = false;
}