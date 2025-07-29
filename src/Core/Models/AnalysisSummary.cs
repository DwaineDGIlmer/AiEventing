
using Core.Contracts;

namespace Core.Models;

/// <summary>
/// Root class representing a comprehensive incident summary, including executive, technical, remediation, and next action details.
/// </summary>
public class AnalysisSummary : IAnalysisSummary
{
    /// <summary>Technical summary with detailed technical reasons and references.</summary>
    public TechnicalSummary TechnicalSummary { get; set; } = new();

    /// <summary> Gets or sets the customer-specific context for the current operation. </summary>
    public KnownIssue KnownIssue { get; set; } = new();

    /// <summary>Gets or sets the high-level executive summary of the incident. </summary>
    public NextActions NextActions { get; set; } = new();
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

    /// <summary>External documentation or references.</summary>
    public List<ExternalReference> ExternalReferences { get; set; } = [];
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

    /// <summary>Gets or sets the list of references related to the analysis, such as documentation or links.</summary>
    public IList<ExternalReference> References { get; set; } = [];
}

/// <summary>
/// Represents a collection of actionable items and related information for follow-up activities.
/// </summary>
/// <remarks>This class provides properties to store lists of technical contacts, vendor contacts,
/// external references,  and customer-related tags. It is intended to organize and manage data required for
/// subsequent actions  in workflows or incident resolution processes.</remarks>
public class NextActions
{
    /// <summary>Gets or sets the next actions to be taken, including technical contacts and follow-up steps.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>List of technical contacts for follow-up.</summary>
    public List<Contact> TechnicalContacts { get; set; } = [];
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
