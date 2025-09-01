using Domain.Customers;

namespace Core.Models
{
    /// <summary>
    /// Represents the context information for a customer, including identifiers, contact details, and additional
    /// attributes.
    /// </summary>
    /// <remarks>This class provides a structured way to store and access customer-related data, such as
    /// unique identifiers,  contact information, and custom attributes. It is commonly used in scenarios where
    /// customer-specific data  needs to be passed between components or persisted.</remarks>
    public sealed class CustomerContext : Customer
    {
        /// <summary>Gets or sets the customer tier, which represents the classification or level of the customer.</summary>
        public string CustomerTier { get; set; } = string.Empty;

        /// <summary>Gets or sets the location of the customer. </summary>
        public string CustomerLocation { get; set; } = string.Empty;

        /// <summary>Gets or sets the time zone associated with the customer.</summary>
        public string CustomerTimeZone { get; set; } = string.Empty;

        /// <summary>Gets or sets the ordered list of workflow names/rules for execution. These rules must exsist in the system.</summary>
        public List<string> ExecutionOrder { get; set; } = [];

        /// <summary>Gets or sets a collection of customer attributes represented as key-value pairs.</summary>
        /// <remarks>This property can be used to store and retrieve custom attributes associated with a
        /// customer. The keys in the dictionary should be unique and meaningful, while the values can represent any
        /// type of data.</remarks>
        public Dictionary<string, object> CustomerAttributes { get; set; } = [];
    }
}
