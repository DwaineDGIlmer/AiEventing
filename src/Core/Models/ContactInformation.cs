namespace Core.Models
{
    /// <summary>
    /// Represents contact information for an individual or entity, including email and phone details.
    /// </summary>
    /// <remarks>This class provides properties to store and retrieve basic contact details such as an email
    /// address and a phone number. Both properties are optional and can be set or left unset depending on the use
    /// case.</remarks>
    public class ContactInformation
    {
        /// <summary>
        /// Gets or sets the email address associated with the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number associated with the entity.
        /// </summary>
        public string Phone { get; set; } = string.Empty;
    }
}
