# Models

This folder contains core data models used throughout the application for representing analysis results, summaries, exceptions, customer context, chat requests/responses, and related entities. These models are designed to provide a structured and consistent way to handle data for incident analysis, reporting, and integration with external services.

---

## Classes

### `AnalysisResultSummary`
Represents a summary of the analysis result, including details such as the unique identifier, customer information, source, fault type, timestamp, and any processing errors encountered. Inherits from `AnalysisSummary`.

**Key Properties:**
- `string Id`: Unique identifier for the analysis result summary.
- `string CustomerId`: Unique identifier for the customer.
- `string Source`: Source of the analysis (origin of the data).
- `string FaultType`: Type of fault associated with the operation or entity.
- `DateTime Timestamp`: When the ingestion request was created.
- `IList<Error>? ProcessingErrors`: Errors encountered during processing.
- Inherits all properties from `AnalysisSummary`.

**Constructors:**
- `AnalysisResultSummary()`: Default constructor.
- `AnalysisResultSummary(IIngestionRequest ingestionRequest)`: Initializes the summary from an ingestion request.

**Usage Example:**
```csharp
var summary = new AnalysisResultSummary(ingestionRequest);
summary.ProcessingErrors = new List<Error> { new Error { ErrorCode = "404", ErrorMessage = "Not found" } };
```

---

### `AnalysisSummary`
Root class representing a comprehensive incident summary, including technical summary, known issues, and next actions.

**Key Properties:**
- `TechnicalSummary TechnicalSummary`: Detailed technical reasons and references.
- `KnownIssue KnownIssue`: Information about known issues.
- `NextActions NextActions`: Follow-up actions and contacts.

---

### `TechnicalSummary`
Provides a detailed technical reason for the incident and external references.

**Key Properties:**
- `string TechnicalReason`
- `List<ExternalReference> ExternalReferences`

---

### `KnownIssue`
Describes a known issue, including details and references.

**Key Properties:**
- `bool IsKnown`
- `string Details`
- `IList<ExternalReference> References`

---

### `NextActions`
Represents actionable items and technical contacts for follow-up.

**Key Properties:**
- `string Description`
- `List<Contact> TechnicalContacts`

---

### `Contact`
Represents a contact person with name, email, and role.

---

### `Error`
Represents an error with a code, message, and optional details.

**Key Properties:**
- `string ErrorCode`
- `string ErrorMessage`
- `IList<string> ErrorDetails`

---

### `SerializableException`
Encapsulates exception details for logging and diagnostics, including type, message, stack trace, and inner exceptions.

---

### `ExceptionContext`
Provides contextual information about an exception, including severity, trace IDs, and custom properties.

---

### `CustomerContext` & `ContactInformation`
Store customer-specific data and contact details.

---

### `OpenAiChatRequest` & `OpenAiChatResponse`
Model the structure of requests and responses for OpenAI chat completions.

---

### `OpenAIEmbeddingModels`
Static class providing constants and utilities for supported OpenAI embedding models and their dimensions.

---

### `ExecutiveSummary`
Represents a high-level summary of an incident, including application, failing components, severity, and causes.

---

### Other Supporting Classes
- `ExternalReference`, `InternalIssue`, `EnvironmentalFailure`, `Cause`, `Usage`, `CompletionChoice`, `TokenDetail`, etc., provide additional structure for incident and chat analysis.

---

## Usage

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

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## License

This project is licensed under the MIT License.

---

## Contacts

For questions or support, please contact dwaine.gilmer at protonmail.com or open an issue on the repository.