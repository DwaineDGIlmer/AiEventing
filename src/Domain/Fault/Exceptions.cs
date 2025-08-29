namespace Domain.Fault
{
    /// <summary>
    /// Represents detailed information about an inner exception, including its type, message, and stack trace.
    /// </summary>
    /// <remarks>This class is typically used to encapsulate information about an exception that occurred
    /// within another exception. It provides additional context for debugging and error analysis.</remarks>
    public class Exceptions
    {
        /// <summary>
        /// Gets or sets the type of the exception represented as a string.
        /// </summary>
        public string ExceptionType { get; set; } = string.Empty;

        /// <summary>
        /// Gets the exception associated with the log event, if any.
        /// </summary>
        /// <value>
        /// An <see cref="Exception"/> instance if an exception occurred; otherwise, <c>null</c>.
        /// </value>
        /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
        public string ExceptionMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the stack trace of the exception.
        /// </summary>
        public string ExceptionStackTrace { get; set; } = string.Empty;
    }
}
