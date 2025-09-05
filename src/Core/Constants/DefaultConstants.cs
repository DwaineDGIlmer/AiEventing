namespace Core.Constants;

/// <summary>
/// Provides default constants for embedding model identifiers used in text processing and machine learning tasks.
/// </summary>
/// <remarks>This class contains predefined string constants representing the names of commonly used
/// embedding models. These constants can be used to reference specific models in applications that require text
/// embeddings.</remarks>
public static class DefaultConstants
{
    #region Mime types
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
    #endregion

    #region Vector DB Constants
    /// <summary>
    /// The base address of the Qdrant service.
    /// </summary>
    /// <remarks>This constant specifies the default URL used to connect to the Qdrant service.  It
    /// includes the protocol, IP address, and port number. Ensure that the service  is accessible at this address
    /// before using it in your application.</remarks>
    public const string BaseAddress = "http://192.168.100.5:6334/";

    /// <summary>
    /// The name of the REST service client name used for making requests to the MCP services.
    /// </summary>
    public const string HttpClientName = "VectorDb_Resilent_Http_ClientName";

    /// <summary>
    /// The host address of the Qdrant service.
    /// </summary>
    /// <remarks>This constant provides the fully qualified domain name (FQDN) of the Qdrant service.
    /// It is used to establish a connection to the Qdrant instance hosted in the specified region.</remarks>
    public const string HostAddress = "bb804cb7-02f8-4aba-af17-5d9158100958.us-east4-0.gcp.cloud.qdrant.io";

    /// <summary>
    /// Represents the endpoint for searching points in the RCA collection within the Qdrant service.
    /// </summary>
    /// <remarks>This constant defines the relative path to the Qdrant API endpoint used for point
    /// search operations. It is intended to be used as part of constructing the full API request URL.</remarks>
    public const string ApiEndpoint = "collections/rca_collection/points/search";

    /// <summary>
    /// The default embedding model identifier used for text embedding operations.
    /// </summary>
    /// <remarks>This constant represents the default model name for embedding tasks.  It can be used to
    /// specify the embedding model when no other model is explicitly provided.</remarks>
    public const string DefaultEmbedding = "text-embedding-3-small";

    /// <summary>
    /// Represents the version of the content format used by the application.
    /// </summary>
    /// <remarks>This constant can be used to identify or validate the version of the content format
    /// expected or produced by the application. The value is immutable and set to "1.0.0".</remarks>
    public const int ContentVersion = 1;

    /// <summary>
    /// The name of the Qdrant collection used for storing RCA (Root Cause Analysis) data.
    /// </summary>
    /// <remarks>This constant defines the default collection name for interacting with the Qdrant
    /// database. It is used to ensure consistency across the application when referencing the RCA
    /// collection.</remarks>
    public const string RcaCollectionName = "rca_collection";

    /// <summary>
    /// Represents the maximum number of items allowed in the collection.
    /// </summary>
    /// <remarks>This constant defines the upper limit for the number of items that can be added to a
    /// collection.  Exceeding this limit may result in an exception or undefined behavior, depending on the
    /// implementation.</remarks>
    public const ulong CollectionLimit = 0;
    #endregion

    #region Connection string
    /// <summary>
    /// Represents the default protocol used for endpoints in a connection string.
    /// </summary>
    /// <remarks>This constant is typically used as a key in connection string parsing or construction to
    /// specify the protocol (e.g., HTTP or HTTPS) for communication with the service.</remarks>
    public const string AzureWebJobsStorage = nameof(AzureWebJobsStorage);
    #endregion

    #region Environment Variable Names
    /// <summary>
    /// Represents the environment variable name for the Qdrant Vector Database API key.
    /// </summary>
    /// <remarks>This constant can be used to retrieve the API key for authenticating with the Qdrant
    /// Vector Database. The value of the environment variable should be set to the API key string.</remarks>
    public const string VECTOR_DB_QDRANT_CERT_THUMBPRINT = nameof(VECTOR_DB_QDRANT_CERT_THUMBPRINT);

    /// <summary>
    /// Represents the environment variable name for the localhost Qdrant Vector Database API key.
    /// </summary>
    /// <remarks>This constant can be used to retrieve the API key for authenticating with the Qdrant
    /// Vector Database. The value of the environment variable should be set to the API key string.</remarks>
    public const string QDRANT_LOCAL_SERVICE_API_KEY = nameof(QDRANT_LOCAL_SERVICE_API_KEY);

    /// <summary>
    /// Represents the environment variable name for the localhost Qdrant Vector Database API key.
    /// </summary>
    /// <remarks>This constant can be used to retrieve the API key for authenticating with the Qdrant
    /// Vector Database. The value of the environment variable should be set to the API key string.</remarks>
    public const string QDRANT_SERVICE_API_KEY = nameof(QDRANT_SERVICE_API_KEY);

    /// <summary>
    /// Represents the environment variable name for the OpenAI API key.
    /// </summary>
    public const string OPENAI_API_KEY = nameof(OPENAI_API_KEY);

    /// <summary>
    /// Represents the environment variable name for the OpenAI URI base address.
    /// </summary>
    public const string AI_API_BASE_ADDRESS = nameof(AI_API_BASE_ADDRESS);

    /// <summary>
    /// Represents the environment variable name for the OpenAI URI endpoint.
    /// </summary>
    public const string AI_API_ENDPOINT = nameof(AI_API_ENDPOINT);

    /// <summary>
    /// Represents the environment variable name for the OpenAI model.
    /// </summary>
    public const string AI_MODEL = nameof(AI_MODEL);

    /// <summary>
    /// Represents the environment variable name for the MCP API key.
    /// </summary>
    public const string MCP_API_KEY = nameof(MCP_API_KEY);

    /// <summary>
    /// Represents the constant name of the API key for the RCA service.
    /// </summary>
    public const string RCASERVICE_API_KEY = nameof(RCASERVICE_API_KEY);

    /// <summary>
    /// Represents the environment variable name for the RCA service API URL.
    /// </summary>
    /// <remarks>This constant holds the name of the environment variable that stores the URL for the RCA
    /// service API. Use this constant to retrieve the API URL from the environment configuration.</remarks>
    public const string RCASERVICE_API_URL = nameof(RCASERVICE_API_URL);
    #endregion
}
