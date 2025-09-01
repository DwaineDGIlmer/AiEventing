using Core.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Core.Helpers
{
    /// <summary>
    /// Provides utility methods for common file system operations.
    /// </summary>
    public static class FileSystemHelpers
    {
        /// <summary>
        /// Determines whether the specified object is null.
        /// </summary>
        /// <param name="obj">The object to check for null.</param>
        /// <param name="length"></param>
        /// <returns><c>true</c> if the object is null; otherwise, <c>false</c>.</returns>
        public static string FileSystemName([NotNullWhen(false)] this string obj, int length = 64)
        {
            if (string.IsNullOrEmpty(obj))
            {
                return string.Empty;
            }
            return SanitizeForFileSystem(obj.GenHashString())[..length];
        }

        /// <summary>
        /// Constructs the file path for a company's profile based on its identifier.
        /// </summary>
        /// <param name="fileName">The unique identifier of the company. Must not be null or empty.</param>
        /// <param name="directory">The directory where the profile files are stored. Must not be null or empty.</param>
        /// <param name="isJson">Indicates whether the file is a JSON file. Defaults to <c>true</c>.</param>
        /// <returns>The full file path to the company's profile JSON file.</returns>
        public static string GetFilePath(string fileName, string directory, bool isJson = true)
        {
            ArgumentNullException.ThrowIfNull(fileName, nameof(fileName));
            ArgumentNullException.ThrowIfNull(directory, nameof(directory));

            if (isJson)
            {
                return Path.Combine(directory, $"{fileName.FileSystemName()}.json");
            }
            return Path.Combine(directory, $"{fileName.FileSystemName()}");
        }

        /// <summary>
        /// Advanced file system string sanitization with multiple options.
        /// </summary>
        /// <param name="input">The input string to sanitize.</param>
        /// <param name="options">Sanitization options.</param>
        /// <returns>A sanitized string safe for file system use.</returns>
        public static string SanitizeForFileSystem(string input, FileSystemSanitizeOptions? options = null)
        {
            options ??= new FileSystemSanitizeOptions();

            if (string.IsNullOrWhiteSpace(input))
                return options.DefaultName;

            string result = input;

            // Convert to lowercase if requested
            if (options.ConvertToLowercase)
                result = result.ToLowerInvariant();

            // Replace spaces if requested
            if (options.ReplaceSpaces)
                result = result.Replace(' ', options.SpaceReplacement);

            // Replace spaces if requested
            if (options.ReplaceColons)
                result = result.Replace(':', options.SpaceReplacement);

            // Handle Unicode normalization
            if (options.NormalizeUnicode)
                result = result.Normalize(NormalizationForm.FormC);

            // Remove or replace accented characters
            if (options.RemoveAccents)
                result = RemoveAccents(result);

            // Get invalid characters
            var invalidChars = Path.GetInvalidFileNameChars()
                .Union(Path.GetInvalidPathChars())
                .Union(options.AdditionalInvalidChars ?? [])
                .ToHashSet();

            var sanitized = new StringBuilder(result.Length);

            foreach (char c in result)
            {
                if (invalidChars.Contains(c) || char.IsControl(c))
                {
                    if (options.RemoveInvalidChars)
                        continue; // Skip invalid characters
                    else
                        sanitized.Append(options.ReplacementChar);
                }
                else
                {
                    sanitized.Append(c);
                }
            }

            result = sanitized.ToString();

            // Handle Windows reserved names
            if (options.HandleReservedNames)
            {
                string[] reservedNames = [ "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3",
            "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3",
            "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" ];

                if (reservedNames.Contains(result.ToUpperInvariant()))
                {
                    result = $"{options.ReservedNamePrefix}{result}";
                }
            }

            // Trim problematic characters
            if (options.TrimProblematicChars)
                result = result.Trim(' ', '.');

            // Handle consecutive replacement characters
            if (options.CollapseReplacements)
            {
                string doubleReplacement = $"{options.ReplacementChar}{options.ReplacementChar}";
                while (result.Contains(doubleReplacement))
                {
                    result = result.Replace(doubleReplacement, options.ReplacementChar.ToString());
                }
            }

            // Ensure we don't have an empty result
            if (string.IsNullOrWhiteSpace(result))
                result = options.DefaultName;

            // Handle length limits
            if (result.Length > options.MaxLength)
            {
                if (options.PreserveExtension)
                {
                    string extension = Path.GetExtension(result);
                    int nameLength = options.MaxLength - extension.Length;
                    if (nameLength > 0)
                    {
                        result = string.Concat(result.AsSpan(0, nameLength), extension);
                    }
                    else
                    {
                        result = result[..options.MaxLength];
                    }
                }
                else
                {
                    result = result[..options.MaxLength];
                }
            }

            return result;
        }

        /// <summary>
        /// Removes accented characters from a string, replacing them with their base equivalents.
        /// </summary>
        private static string RemoveAccents(string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Configuration options for file system string sanitization.
        /// </summary>
        sealed public class FileSystemSanitizeOptions
        {
            /// <summary>
            /// Gets or sets the character used to replace invalid or restricted characters in a string.
            /// </summary>
            public char ReplacementChar { get; set; } = '_';

            /// <summary>
            /// Gets or sets the maximum allowable length for the input.
            /// </summary>
            public int MaxLength { get; set; } = 255;

            /// <summary>
            /// Gets or sets the default name used when no specific name is provided.
            /// </summary>
            public string DefaultName { get; set; } = "unnamed";

            /// <summary>
            /// Gets or sets a value indicating whether the input should be converted to lowercase.
            /// </summary>
            public bool ConvertToLowercase { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether spaces should be replaced in the processed text.
            /// </summary>
            public bool ReplaceSpaces { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether spaces should be replaced in the processed text.
            /// </summary>
            public bool ReplaceColons { get; set; } = true;

            /// <summary>
            /// Gets or sets the character used to replace spaces in a string.
            /// </summary>
            public char SpaceReplacement { get; set; } = '_';

            /// <summary>
            /// Gets or sets a value indicating whether accents should be removed from text.
            /// </summary>
            public bool RemoveAccents { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether Unicode normalization should be applied.
            /// </summary>
            public bool NormalizeUnicode { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether invalid characters should be removed from the input.
            /// </summary>
            public bool RemoveInvalidChars { get; set; } = false;

            /// <summary>
            /// Gets or sets a value indicating whether reserved names should be handled specially.
            /// </summary>
            public bool HandleReservedNames { get; set; } = true;

            /// <summary>
            /// Gets or sets the prefix used for reserved names.
            /// </summary>
            public string ReservedNamePrefix { get; set; } = "_";

            /// <summary>
            /// Gets or sets a value indicating whether problematic characters should be trimmed from input strings.
            /// </summary>
            public bool TrimProblematicChars { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether replacements should be collapsed into a single operation.
            /// </summary>
            public bool CollapseReplacements { get; set; } = true;

            /// <summary>
            /// Gets or sets a value indicating whether the file extension should be preserved during processing.
            /// </summary>
            public bool PreserveExtension { get; set; } = true;

            /// <summary>
            /// Gets or sets an array of additional characters considered invalid for the current context.
            /// </summary>
            public char[]? AdditionalInvalidChars { get; set; }
        }
    }
}
