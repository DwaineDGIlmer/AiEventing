namespace Domain.Customers;

/// <summary>
/// Represents a customer and their associated details, including identifiers, contact information, and role.
/// </summary>
/// <remarks>This class provides properties to store information about a customer, such as their unique
/// identifiers, name, email, and role. It is typically used to manage customer-related data in applications that
/// require customer identification and contact details.</remarks>
public class Customer
{
    /// <summary>Unique identifier for the incident.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the name of the customer.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Role of the contact person.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Email address of the contact.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the unique identifier for the account.</summary>
    public string AccountId { get; set; } = string.Empty;
}
