using Core.Contracts;

namespace Core.Models
{
    /// <summary>
    /// Represents an error with a code, message, and optional details.
    /// </summary>
    public class Error : IError
    {
        /// <summary>
        /// Gets the timestamp representing the current date and time in UTC, formatted as an ISO 8601 string.
        /// </summary>
        public string TimeStamp { get; } = DateTime.UtcNow.ToString("o");

        /// <summary>
        /// Gets or sets an error code.
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a human-readable error message.
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a map of additional details associated with the error.
        /// </summary>
        public IList<string> ErrorDetails { get; set; } = [];
    }
}
