namespace Core.Constants
{
    /// <summary>
    /// Provides default values and settings used throughout the application.
    /// </summary>
    /// <remarks>This class contains constants and static members that represent commonly used default values.
    /// It is intended to centralize default configurations to ensure consistency across the application.</remarks>
    public static class Defaults
    {
        /// <summary>
        /// Represents the MIME type for CSV (Comma-Separated Values) files.
        /// </summary>
        /// <remarks>This constant can be used to specify or identify the MIME type for CSV files in HTTP
        /// headers, file uploads, or other contexts where MIME types are required.</remarks>
        public const string CsvMimeType = "text/csv";

        /// <summary>
        /// Represents the MIME type for JSON content.
        /// </summary>
        /// <remarks>This constant can be used to specify or compare the MIME type for JSON data in HTTP
        /// requests or responses.</remarks>
        public const string JsonMimeType = "application/json";
    }
}
