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

        #region Resilent http policies
        /// <summary>
        /// Represents the default HttpTimeout for the http client.
        /// </summary>
        public const int HttpTimeout = 60;

        /// <summary>
        /// Represents the duration of break when the circuit is open.
        /// </summary>
        public const int DurationOfBreak = 30;
        #endregion

        #region Open AI Model Identifiers
        /// <summary>
        /// Represents the identifier for the OpenAI GPT-4 Turbo model.
        /// </summary>
        /// <remarks>This constant can be used to specify the model name when interacting with OpenAI's
        /// API.</remarks>
        public const string OpenAiModel = "gpt-3.5-turbo";

        /// <summary>
        /// The name of the resilient HTTP client used for interacting with the OpenAI API.
        /// </summary>
        /// <remarks>This constant is typically used as a key or identifier when configuring or retrieving the
        /// resilient HTTP client for OpenAI-related operations.</remarks>
        public const string OpenAiClientName = "OpenAi_Resilent_Http_ClientName";

        /// <summary>
        /// Gets or sets the base URL of the API used for making requests to AI service.
        /// </summary>
        public const string OpenAiABaseAddress = "https://api.openai.com";

        /// <summary>
        /// Represents the API endpoint for chat completions in version 1.
        /// </summary>
        /// <remarks>This constant defines the relative path to the chat completions endpoint. It is typically
        /// used to construct full API URLs when interacting with the service.</remarks>
        public const string OpenAiEndpoint = "chat/completions";
        #endregion

        #region Open SerpApi Identifiers
        /// <summary>
        /// The name of the resilient HTTP client used for interacting with the OpenAI API.
        /// </summary>
        /// <remarks>This constant is typically used as a key or identifier when configuring or retrieving the
        /// resilient HTTP client for OpenAI-related operations.</remarks>
        public const string SerpApiClientName = "SerpApi_Resilent_Http_ClientName";

        /// <summary>
        /// Gets or sets the base URL of the API used for making requests to AI service.
        /// </summary>
        public const string SerpApiBaseAddress = "https://serpapi.com";

        /// <summary>
        /// Represents the API endpoint for chat completions in version 1.
        /// </summary>
        /// <remarks>This constant defines the relative path to the chat completions endpoint. It is typically
        /// used to construct full API URLs when interacting with the service.</remarks>
        public const string SearchEndpoint = "/search.json";

        /// <summary>
        /// Represents the query string used for searching data engineers.
        /// </summary>
        public const string SerpApiQuery = "Data Engineer";

        /// <summary>
        /// Represents the location as a constant string.
        /// </summary>
        public const string SerpApiLocation = "Charlotte, NC";

        /// <summary>
        /// Represents the directory name where company profile files are stored.
        /// </summary>
        public const string FileCompanyProfileDirectory = "app_data/company_profiles";

        /// <summary>
        /// Represents the directory name where job summary files are stored.
        /// </summary>
        public const string FileJobProfileDirectory = "app_data/job_summaries";
        #endregion
    }
}
