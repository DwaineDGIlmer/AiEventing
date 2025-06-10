namespace Core.Models;

/// <summary>
/// Root class representing a comprehensive incident summary, including executive, technical, remediation, and next action details.
/// </summary>
public class AnalysisSummary
{
    /// <summary>
    /// Gets or sets the customer-specific context for the current operation.
    /// </summary>
    public CustomerContext CustomerContext { get; set; } = new CustomerContext();

    /// <summary>
    /// High-level executive summary of the incident.
    /// </summary>
    public ExecutiveSummary ExecutiveSummary { get; set; } = new ExecutiveSummary();

    /// <summary>
    /// Technical summary with detailed technical reasons and references.
    /// </summary>
    public TechnicalSummary TechnicalSummary { get; set; } = new TechnicalSummary();

    /// <summary>
    /// Summary of remediation steps taken.
    /// </summary>
    public RemediationSummary RemediationSummary { get; set; } = new RemediationSummary();

    /// <summary>
    /// Next actions, including contacts and references.
    /// </summary>
    public NextActions NextActions { get; set; } = new NextActions();
}

/// <summary>
/// Represents a collection of performance metrics, such as response time and error rate,  for monitoring and
/// analyzing system behavior.
/// </summary>
/// <remarks>This class provides properties to store key metrics related to system performance.  Metrics
/// such as response time and error rate can be used to evaluate the efficiency  and reliability of an application
/// or service.</remarks>
public class Metrics
{
    /// <summary>Response time metric (e.g., "200ms").</summary>
    public string ResponseTime { get; set; } = string.Empty;

    /// <summary>Error rate metric (e.g., "5%").</summary>
    public string ErrorRate { get; set; } = string.Empty;
}

/// <summary>
/// Represents a summary of technical details related to an incident, including the root cause, stack trace
/// references, external documentation, known issues, and exception tags.
/// </summary>
/// <remarks>This class is designed to provide a comprehensive overview of the technical aspects of an
/// incident, enabling detailed analysis and troubleshooting. It includes information such as the technical reason
/// for the incident, references to stack trace locations, links to external documentation, known issues, and tags
/// associated with exceptions.</remarks>
public class TechnicalSummary
{
    /// <summary>Detailed technical reason for the incident.</summary>
    public string TechnicalReason { get; set; } = string.Empty;

    /// <summary>References to stack trace locations.</summary>
    public List<SerializableException> StackTraceReferences { get; set; } = [];

    /// <summary>External documentation or references.</summary>
    public List<ExternalReference> ExternalReferences { get; set; } = [];

    /// <summary>Information about known issues.</summary>
    public KnownIssue KnownIssue { get; set; } = new();

    /// <summary>Tags for exceptions involved in the incident.</summary>
    public List<string> ExceptionTags { get; set; } = [];
}

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
}

/// <summary>
/// Represents a summary of remediation actions taken to address issues or concerns.
/// </summary>
/// <remarks>This class provides a collection of remediation steps, allowing users to review or process 
/// the actions performed. Each remediation step is represented as an instance of the <see cref="Remediation"/>
/// class.</remarks>
public class RemediationSummary
{
    /// <summary>List of remediation steps taken.</summary>
    public List<Remediation> Remediations { get; set; } = [];
}

/// <summary>
/// Represents a remediation step for addressing an issue or vulnerability.
/// </summary>
/// <remarks>A remediation typically includes a description of the action to be taken, the type of
/// remediation,  whether the issue was preventable, and any resolution provided by the vendor.</remarks>
public class Remediation
{
    /// <summary>SuggestedFix of the remediation step.</summary>
    public string SuggestedFix { get; set; } = string.Empty;

    /// <summary>Type of remediation (e.g., Code Patch).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Indicates if the remediation was preventable.</summary>
    public bool Preventable { get; set; }

    /// <summary>Vendor's resolution, if applicable.</summary>
    public string VendorResolution { get; set; } = string.Empty;
}

/// <summary>
/// Represents a collection of actionable items and related information for follow-up activities.
/// </summary>
/// <remarks>This class provides properties to store lists of technical contacts, vendor contacts,
/// external references,  and customer-related tags. It is intended to organize and manage data required for
/// subsequent actions  in workflows or incident resolution processes.</remarks>
public class NextActions
{
    /// <summary>List of technical contacts for follow-up.</summary>
    public List<Contact> TechnicalContacts { get; set; } = [];

    /// <summary>List of vendor contacts for follow-up.</summary>
    public List<Contact> VendorContacts { get; set; } = [];

    /// <summary>External references for next actions.</summary>
    public List<ExternalReference> ExternalReferences { get; set; } = [];

    /// <summary>Tags related to the customer or incident.</summary>
    public List<string> CustomerTags { get; set; } = [];
}

/// <summary>
/// Represents a contact person with associated details such as name, email, and role.
/// </summary>
/// <remarks>This class is typically used to store and manage information about individuals in a system,
/// such as team members, clients, or other relevant contacts.</remarks>
public class Contact
{
    /// <summary>Name of the contact person.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Email address of the contact.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Role of the contact person.</summary>
    public string Role { get; set; } = string.Empty;
}
