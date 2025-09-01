using Core.Helpers;
using Domain.Fault;

namespace Core.Models
{
    /// <summary>
    /// Represents detailed information about an inner exception, including its type, message, and stack trace.
    /// </summary>
    /// <remarks>This class is typically used to encapsulate information about an exception that occurred
    /// within another exception. It provides additional context for debugging and error analysis.</remarks>
    public sealed class SerializableException : Exceptions
    {
        /// <summary>
        /// Gets or sets the collection of inner exceptions associated with the current exception.
        /// </summary>
        public IList<SerializableException> InnerExceptions { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableException"/> class.
        /// </summary>
        public SerializableException() { }

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

        /// <summary>
        /// Returns a string representation of the exception details, including the exception type, message, stack
        /// trace,  and any inner exceptions.
        /// </summary>
        /// <remarks>The returned string includes the exception type, message, and stack trace, formatted
        /// for readability.  If there are inner exceptions, they are included in the output as well, separated by new
        /// lines.</remarks>
        /// <returns>A string containing the formatted details of the exception and its inner exceptions, if any.</returns>
        override public string ToString()
        {
            return new string[]
            {
                $"Exception Type: {ExceptionType}",
                $"Message: {ExceptionMessage}",
                $"Stack Trace: {ExceptionStackTrace}",
                InnerExceptions.Count > 0 ? "Inner Exceptions:" : string.Empty,
                string.Join(Environment.NewLine, InnerExceptions.Select(e => e.ToString()))
            }.Where(s => !string.IsNullOrWhiteSpace(s)).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}");
        }
    }
}
