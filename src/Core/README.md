# 🧩 Core

The **Core** project contains the fundamental building blocks and reusable code for the AIEventing solution. It is designed to be independent of other layers and provides shared logic, domain models, and abstractions that can be referenced by other assemblies.

---

## 🏗️ Purpose

- Provides interfaces and abstractions for use by other layers (such as Application and Infrastructure).
- Contains reusable utilities and helper classes that are not dependent on external frameworks.
- Keep this project free of dependencies on other solution layers or external technologies.
- Only include code that is intended to be shared and reused across multiple projects.
- Maintain a clear separation of concerns and avoid implementation details specific to other layers.

---

## ✨ Features

### 🗄️ Caching

See [Caching/README.md](./Caching/README.md) for details.

- **MemoryCacheService**: An implementation of `ICacheService` using `IMemoryCache` for in-memory caching. Supports async operations for retrieving, storing, and removing cached items. Can be enabled or disabled via constructor parameter.
- **FileCaching**: File-based caching for persistence across application restarts.

### ⚙️ Configuration

See [Configuration/README.md](./Configuration/README.md) for details.

- Provides a configuration management system for the application, supporting settings for OpenAI, HTTP resilience, retry, circuit breaker, and more.
- Supports loading from JSON, environment variables, and other providers.

### 📜 Contracts

See [Contracts/README.md](./Contracts/README.md) for details.

- Defines interfaces and abstractions used across the suite of applications, such as `ICacheService`, `IAiChatService`, `IEmbeddingService`, and more.

### 🧩 Extensions

See [Extensions/README.md](./Extensions/README.md) for details.

- Extension methods for common .NET types and solution-specific utilities, including hashing, serialization, HTTP helpers, null checking, and service registration.

### 🛠️ Helpers

See [Helpers/README.md](./Helpers/README.md) for details.

- Utility classes for reflection, JSON validation, file system operations, error creation, exception handling, caching, and regular expressions.

### 📦 Models

See [Models/README.md](./Models/README.md) for details.

- Domain entities and value objects used throughout the solution, including analysis summaries, error models, chat requests/responses, customer context, and more.

### 📝 Serializers

- Abstractions and helpers for serialization (planned/extend as needed).

### 🧠 Services

See [Services/README.md](./Services/README.md) for details.

- Interfaces and base implementations for core services, including AI chat, embedding, and fault analysis services.

---

## 📁 Folder Structure

```
Core/
├── Caching/
│   └── MemoryCacheService.cs
├── Contracts/
│   └── ICacheService.cs
├── Extensions/
├── Helpers/
├── Models/
├── Serializers/
├── Services/
└── Core.csproj
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