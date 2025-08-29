namespace Domain.Incident;

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
