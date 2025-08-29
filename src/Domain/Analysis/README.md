# 🧠 Analysis Domain Models

This folder contains core data models used throughout the application for representing analysis results, summaries, exceptions, customer context, chat requests/responses, and related entities. These models provide a structured and consistent way to handle data for incident analysis, reporting, and integration with external services.

---

## 📦 Classes

### 🛠️ `TechnicalSummary`
Provides a detailed technical reason for the incident and external references.

**Key Properties:**
- `string TechnicalReason`
- `List<ExternalReference> ExternalReferences`

---

### 🐞 `KnownIssue`
Describes a known issue, including details and references.

**Key Properties:**
- `bool IsKnown`
- `string Details`
- `IList<ExternalReference> References`

---

### 🏃 `NextActions`
Represents actionable items and technical contacts for follow-up.

**Key Properties:**
- `string Description`
- `List<Contact> TechnicalContacts`

---

### 👤 `Contact`
Represents a contact person with name, email, and role.

**Key Properties:**
- `string Name`
- `string Email`
- `string Role`

---

### ❗ `Errors`
Represents an error with a code, message, and optional details.

**Key Properties:**
- `string ErrorCode`
- `string ErrorMessage`
- `IList<string> ErrorDetails`
- `string TimeStamp`

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