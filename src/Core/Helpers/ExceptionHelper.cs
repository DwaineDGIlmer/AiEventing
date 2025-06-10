using Core.Models;
using Core.Serializers;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers
{
    /// <summary>
    /// Generates a unique hash for the specified exception based on its type, message, and stack trace.
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// Generates a unique hash for the specified exception based on its type, message, and stack trace.
        /// </summary>
        /// <param name="ex">The exception to generate the hash for. Cannot be <see langword="null"/>.</param>
        /// <returns>A hexadecimal string representing the SHA-256 hash of the exception's details.</returns>
        public static string GetExceptionHash(SerializableException ex)
        {
            // Combine exception type, message, and stack trace for uniqueness.
            // If all these are the same, the hash will be the same.
            var input = $"{ex.ExceptionType}|{ex.ExceptionMessage}|{ex.ExceptionStackTrace}|{JsonConvertService.Instance.Serialize(ex.InnerExceptions)}";
            return GetExceptionHash(input);
        }

        /// <summary>
        /// Generates a unique hash for the specified exception based on its type, message, and stack trace.
        /// </summary>
        /// <param name="ex">The exception to generate the hash for. Cannot be <see langword="null"/>.</param>
        /// <returns>A hexadecimal string representing the SHA-256 hash of the exception's details.</returns>
        public static string GetExceptionHash(Exception ex)
        {
            // Combine exception type, message, and stack trace for uniqueness.
            // If all these are the same, the hash will be the same.
            var input = $"{ex.GetType().FullName}|{ex.Message}|{ex.StackTrace}";
            return GetExceptionHash(input);
        }

        /// <summary>
        /// Computes a SHA-256 hash for the given input string.
        /// </summary>
        /// <remarks>This method generates a hash based on the provided input string using the SHA-256
        /// algorithm. The resulting hash can be used to uniquely identify the input, such as for exception tracking or
        /// other scenarios requiring deterministic hashing.</remarks>
        /// <param name="input">The input string to hash. Cannot be null or empty.</param>
        /// <returns>A hexadecimal string representation of the computed hash.</returns>
        public static string GetExceptionHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            // Combine exception type, message, and stack trace for uniqueness.
            // If all these are the same, the hash will be the same.
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash);
        }

        /// <summary>
        /// Retrieves a list of inner exceptions from the specified exception.
        /// </summary>
        /// <remarks>This method iterates through the inner exception chain of the provided exception, 
        /// starting with the specified exception itself, and creates an <see cref="SerializableException"/>  representation for
        /// each exception in the chain.</remarks>
        /// <param name="exception">The exception from which to extract inner exceptions. Must not be <see langword="null"/>.</param>
        /// <returns>A list of inner exceptions represented as <see cref="SerializableException"/> objects.  If the specified exception has
        /// no inner exceptions, an empty list is returned.</returns>
        public static IList<SerializableException> GetInnerExceptions(Exception? exception = null)
        {
            var innExceptionList = new List<SerializableException>();
            if (exception is null)
            {
                return innExceptionList;
            }

            Exception? innerExcption = exception.InnerException;
            while (innerExcption is not null)
            {
                innExceptionList.Add(new SerializableException(innerExcption));
                innerExcption = innerExcption?.InnerException;
            }
            return innExceptionList;
        }
    }
}
