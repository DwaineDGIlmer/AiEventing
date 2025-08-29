namespace Domain.Analysis;

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
}
