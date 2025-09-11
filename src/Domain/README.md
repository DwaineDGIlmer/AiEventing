# Domain

The **Domain** project contains the fundamental building blocks and reusable code for the AIEventing solution. It is designed to be independent of other layers and provides shared logic, domain models, and abstractions that can be referenced by other assemblies. 

---

## 🏗️ Purpose

- Houses domain entities, value objects, and core business logic.
- Provides interfaces and abstractions for use by other layers (such as Application and Infrastructure).
- Contains reusable utilities and helper classes that are not dependent on external frameworks.
- Keep this project free of dependencies on other solution layers or external technologies.
- Only include code that is intended to be shared and reused across multiple projects.

---

## ✨ Features

### 📊 Analysis

See [Analysis/README.md](./Analysis/README.md) for details.

- Core domain entities and value objects related to analysis, such as `AnalysisSummary`, `AnalysisDetail`, and `AnalysisResult`.
- Business logic for processing and summarizing analysis results.
- Utilities for working with analysis data.

### 👥 Customers

See [Customers/README.md](./Customers/README.md) for details.

- Domain entities and value objects related to customers, such as `CustomerContext` and `CustomerSettings`.
- Business logic for managing customer information and settings.

### ⚡ Fault

See [Fault/README.md](./Fault/README.md) for details.

- Domain entities and value objects related to faults, such as `FaultEvent`, `FaultDetail`, and `FaultSummary`.
- Business logic for processing and summarizing fault events.
- Utilities for working with fault data.

### 🚨 Incident

See [Incident/README.md](./Incident/README.md) for details.

- Domain entities and value objects related to incidents, such as `IncidentReport`, `IncidentDetail`, and `IncidentSummary`.
- Business logic for managing and summarizing incident reports.
- Utilities for working with incident data.

---

## 📁 Folder Structure

```
Domain/
├── Analysis/
├── Customers/
├── Fault/
├── Incident/
├── README.md
```

---

## 🤝 Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## 📄 License

This project is licensed under the MIT License.

---

## 📬 Contacts

For questions or support, please contact dwaine.gilmer at protonmail.com or open an issue on the repository.