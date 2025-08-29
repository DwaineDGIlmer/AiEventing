# 🧠 Incident Domain Models

This folder contains domain models specifically used for incident analysis and reporting. These classes provide a structured way to represent executive summaries, internal and external issues, environmental failures, and the underlying causes of incidents.

---

## 📦 Classes

### `ExecutiveSummary`
A high-level summary of an incident, including:
- Unique identifier (`Id`)
- Incident summary (`Summary`)
- Application and failing component
- Severity and high-level cause
- Lists of internal issues, external issues, and environmental failures
- Detailed cause information

### `InternalIssue`
Represents an internal issue within an incident.
- `Description`: Details of the issue
- `Preventable`: Indicates if the issue was preventable

### `ExternalIssue`
Represents an external issue related to an incident.
- `Description`: Details of the issue
- `Type`: Type of external issue (e.g., Vendor)
- `Details`: Additional information about the external issue

### `EnvironmentalFailure`
Represents an environmental failure, such as a power outage or external disruption.
- `Type`: Type of environmental failure
- `Description`: Details of the failure

### `Cause`
Details about the responsible entity or factor for an incident.
- `WhoOrWhat`: Who or what caused the incident
- `Preventable`: Indicates if the cause was preventable

---

## 📖 Usage

These models are used throughout the application for:
- Structuring and storing incident summaries and details
- Reporting and analyzing the root causes of incidents
- Tracking internal, external, and environmental factors contributing to incidents

**Example: Creating an executive summary**
```csharp
var executiveSummary = new ExecutiveSummary
{
    Id = Guid.NewGuid().ToString(),
    Summary = "Major outage affecting payment processing.",
    Application = "PaymentService",
    FailingComponent = "TransactionProcessor",
    Severity = "High",
    HighLevelCause = "Database connection failure",
    InternalIssues = new List<InternalIssue>
    {
        new InternalIssue { Description = "Missing retry logic", Preventable = true }
    },
    ExternalIssues = new List<ExternalIssue>
    {
        new ExternalIssue { Description = "Third-party API downtime", Type = "Vendor", Details = "Payment gateway unavailable" }
    },
    EnvironmentalFailures = new List<EnvironmentalFailure>
    {
        new EnvironmentalFailure { Type = "Power Outage", Description = "Data center lost power" }
    },
    Cause = new Cause { WhoOrWhat = "Database server", Preventable = false }
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