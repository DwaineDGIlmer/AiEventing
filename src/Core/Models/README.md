# 🧠 Models

This folder contains core data models used throughout the application for representing analysis results, summaries, exceptions, customer context, chat requests/responses, and related entities. These models provide a structured and consistent way to handle data for incident analysis, reporting, and integration with external services.

---

## 📦 Classes

### 📊 `AnalysisResultSummary`
Represents a summary of the analysis result, including details such as the unique identifier, customer information, source, fault type, timestamp, and any processing errors encountered. Inherits from [`Core.Models.AnalysisSummary`](AnalysisSummary.cs).

**Key Properties:**
- `string Id`: Unique identifier for the analysis result summary.
- `string CustomerId`: Unique identifier for the customer.
- `string Source`: Source of the analysis (origin of the data).
- `string FaultType`: Type of fault associated with the operation or entity.
- `DateTime Timestamp`: When the ingestion request was created.
- `IList<Core.Models.Error>? ProcessingErrors`: Errors encountered during processing.
- Inherits all properties from [`Core.Models.AnalysisSummary`](AnalysisSummary.cs).

**Constructors:**
- `AnalysisResultSummary()`: Default constructor.
- `AnalysisResultSummary(IIngestionRequest ingestionRequest)`: Initializes the summary from an ingestion request.

---

### 📝 `AnalysisSummary`
Root class representing a comprehensive incident summary, including technical summary, known issues, and next actions.

**Key Properties:**
- `TechnicalSummary TechnicalSummary`: Detailed technical reasons and references.
- `KnownIssue KnownIssue`: Information about known issues.
- `NextActions NextActions`: Follow-up actions and contacts.

---

### 🗄️ `CacheEntry`
Represents a cache entry with a key, value, and optional expiration time.

**Key Properties:**
- `string Value`: The cached value (serialized).
- `string ValueTypeName`: The type name of the cached value.
- `Type ValueType`: The runtime type of the cached value.
- `DateTimeOffset? AbsoluteExpiration`: Optional expiration time for the cache entry.
- `TimeSpan? AbsoluteExpirationRelativeToNow`: Optional relative expiration time.
- `object Key`: The cache key.
- `CacheItemPriority Priority`: Cache item priority.
- `long? Size`: Size of the cache entry.
- `TimeSpan? SlidingExpiration`: Optional sliding expiration time.

---

### 💬 `ChatAnalysisSummary`
Root class for incident summary in chat context, including technical summary, known issues, next actions, confidence score, and references.

**Key Properties:**
- `string TechnicalSummary`
- `string KnownIssue`
- `string NextActions`
- `decimal ConfidenceScore`
- `IList<string> References`
- `AnalysisResultSummary AnalysisSummaryResult`
- `AnalysisSummary AnalysisSummary`

---

### 👤 `CustomerContext`
Stores customer-specific data and contact details.

**Key Properties:**
- `string CustomerTier`
- `string CustomerLocation`
- `string CustomerTimeZone`
- `List<string> ExecutionOrder`
- `Dictionary<string, object> CustomerAttributes`

---

### ❗ `Error`
Represents an error with a code, message, and optional details.

**Key Properties:**
- Inherits from domain `Errors` and implements `IError`.

---

### 🗂️ `ExceptionContext`
Provides contextual information about an exception, including severity, trace IDs, and custom properties.

**Key Properties:**
- `string Id`
- `string Application`
- `DateTimeOffset Timestamp`
- `LogLevel SeverityLevel`
- `string FaultId`
- `string TraceId`
- `string SpanId`
- `SerializableException? Exception`
- `string ExceptionType`
- `string StackTrace`
- `string Service`
- `string Environment`
- `string SourceFile`
- `int LineNumber`
- `string Message`
- `string Source`
- `string Method`
- `string TargetSite`
- `IList<SerializableException> InnerException`
- `int ThreadId`
- `string ExceptionId`
- `string ExceptionOccurrenceId`
- `string? CorrelationId`
- `string Otelschema`
- `IDictionary<string, object> CustomProperties`

---

### 🏢 `FaultAnalysisContext`
Aggregates analysis, exception, and customer context for fault analysis.

**Key Properties:**
- `ExceptionContext ExceptionContext`
- `CustomerContext CustomerContext`

---

### 🧠 `OpenAIEmbeddingModels`
Static class providing constants and utilities for supported OpenAI embedding models and their dimensions.

**Key Properties:**
- Model name constants (e.g., `Embedding3Small`, `TextEmbedding3Large`)
- `Dictionary<string, ulong> ModelDimensions`
- `static ulong GetDimension(string model)`

---

### 🏷️ `RestApiSettings`
Abstract class for OpenAI API configuration, including base URL, API key, endpoint, and HTTP client name.

**Key Properties:**
- `bool IsEnabled`
- `bool IsCachingEnabled`
- `string? ApiKey`
- `string BaseAddress`
- `string Endpoint`
- `string HttpClientName`

---

### 🪲 `SerializableException`
Represents a serializable exception with details such as message, type, stack trace, and inner exceptions.

**Key Properties:**
- `IList<SerializableException> InnerExceptions`

---

## 🧾 Other Supporting Classes

- `ExternalReference`, `InternalIssue`, `EnvironmentalFailure`, `Cause`, `Usage`, `CompletionChoice`, `TokenDetail`, etc., provide additional structure for incident and chat analysis.

---

## 📖 Usage

These models are used throughout the application for:
- Storing and transferring analysis results and summaries.
- Structuring requests and responses for AI and chat services.
- Capturing exception and error details for diagnostics and reporting.

**Example: Creating and populating an analysis summary**
```csharp
var summary = new AnalysisResultSummary
{
    Id = Guid.NewGuid().ToString(),
    CustomerId = "customer-123",
    Source = "API",
    FaultType = "ValidationError",
    Timestamp = DateTime.UtcNow,
    TechnicalSummary = new TechnicalSummary
    {
        TechnicalReason = "Null reference exception",
        ExternalReferences = new List<ExternalReference>
        {
            new ExternalReference { Type = "Documentation", Url = "https://docs.example.com", Description = "API docs" }
        }
    },
    KnownIssue = new KnownIssue { IsKnown = true, Details = "Known bug in v1.2.3" },
    NextActions = new NextActions { Description = "Contact support", TechnicalContacts = new List<Contact>() }
};
```

---

## 🤝 Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.  
See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

---

## 📄 License

This project is licensed under the [MIT License](../../LICENSE).

---

## 📬 Contact

For questions or support, please contact Dwaine Gilmer at [Protonmail.com](mailto:dwaine.gilmer@protonmail.com) or submit an issue on [the project's GitHub repository](https://github.com/your-org/your-repo).