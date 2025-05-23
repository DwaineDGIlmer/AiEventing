using System.Security.Cryptography;
using System.Text;

namespace Core.Models
{
    /// <summary>
    /// Generates a unique hash for the specified exception based on its type, message, and stack trace.
    /// </summary>
    internal static class ExceptionHashGenerator
    {
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
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(hash);
        }
    }
}
