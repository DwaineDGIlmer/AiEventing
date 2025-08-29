namespace Domain.Incident;

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
