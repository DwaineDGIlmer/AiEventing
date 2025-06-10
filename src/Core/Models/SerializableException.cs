using Core.Helpers;
using System.Diagnostics;

namespace Core.Models
{
    /// <summary>
    /// Represents detailed information about an inner exception, including its type, message, and stack trace.
    /// </summary>
    /// <remarks>This class is typically used to encapsulate information about an exception that occurred
    /// within another exception. It provides additional context for debugging and error analysis.</remarks>
    public class SerializableException
    {
        /// <summary>
        /// Gets or sets the type of the exception represented as a string.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets the exception associated with the log event, if any.
        /// </summary>
        /// <value>
        /// An <see cref="Exception"/> instance if an exception occurred; otherwise, <c>null</c>.
        /// </value>
        /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
        public string ExceptionMessage { get; set; }

        /// <summary>
        /// Gets the stack trace associated with the log event, if available.
        /// </summary>
        /// <value>
        /// An optional <see cref="StackTrace"/> object that provides information about the call stack at the time the event was logged, or <c>null</c> if not applicable.
        /// </value>
        /// <remarks>Should follow https://www.w3.org/TR/trace-context/#relationship-between-the-headers guidance.</remarks>
        public string ExceptionStackTrace { get; set; }

        /// <summary>
        /// Gets or sets the collection of inner exceptions associated with the current exception.
        /// </summary>
        public IList<SerializableException> InnerExceptions { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableException"/> class.
        /// </summary>
        /// <remarks>This constructor initializes the <see cref="ExceptionType"/>, <see
        /// cref="ExceptionMessage"/>,  and <see cref="ExceptionStackTrace"/> properties to empty strings.</remarks>
        public SerializableException()
        {
            ExceptionType = string.Empty;
            ExceptionMessage = string.Empty;
            ExceptionStackTrace = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableException"/> class, extracting details from the specified
        /// exception.
        /// </summary>
        /// <remarks>This constructor extracts the type, message, and stack trace from the provided
        /// exception. If the exception's properties are <see langword="null"/>, default values are used.</remarks>
        /// <param name="exception">The exception from which to extract details. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="exception"/> is <see langword="null"/>.</exception>
        public SerializableException(Exception exception)
        {
            ExceptionType = exception?.GetType().FullName ?? string.Empty;
            ExceptionMessage = exception?.Message ?? string.Empty;
            ExceptionStackTrace = exception?.StackTrace ?? string.Empty;
            InnerExceptions = ExceptionHelper.GetInnerExceptions(exception);
        }
    }
}
