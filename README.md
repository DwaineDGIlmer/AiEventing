# 🧠 AIEventing

AIEventing is a .NET solution for advanced, AI-assisted event logging and fault analysis. It provides structured logging, resilient HTTP clients, and integration with AI models (such as GPT-4) to analyze exceptions and stack traces, offering actionable insights for developers.

---

## ✨ Features

- **Structured Logging:**  
  Customizable, scoped, and structured logging with support for multiple publishers and OpenTelemetry-compliant log events.

- **AI-Powered Fault Analysis:**  
  Integrates with AI models (e.g., GPT-4) to analyze .NET stack traces and suggest fixes.

- **Resilient HTTP Clients:**  
  Uses Polly for retry, circuit breaker, and bulkhead policies to ensure robust communication with external services.

- **Extensible Architecture:**  
  Easily add new log publishers, fault analysis providers, or serialization strategies.

- **Asynchronous, Thread-Safe Publishing:**  
  Log events are published asynchronously for high throughput and minimal application impact.

- **Pluggable Publisher Model:**  
  Swap or combine log output destinations (console, EventSource, file, remote, etc.) by implementing the `IPublisher` interface.

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 or later](https://dotnet.microsoft.com/download)
- An API key for your AI model provider (e.g., OpenAI)

### Installation

Clone the repository:

```sh
git clone https://github.com/your-org/AIEventing.git
cd AIEventing
```

Restore dependencies:

```sh
dotnet restore
```

---

## 📝 Usage


### Configuration


AIEventing requires the following environment variables for AI integration. These are read at runtime and override configuration values if present:

- `OPENAI_API_KEY` – Your AI model API key (used for OpenAI)
- `OPENAI_API_BASE_ADDRESS` – The base address for the AI API (used for RCA service integration)
- `OPENAI_API_ENDPOINT` – The endpoint for the AI API (used for OpenAI integration)
- `OPENAI_MODEL` – The model name (e.g., `gpt-4`) for OpenAI
- `RCASERVICE_API_KEY` – API key specifically for RCA (Root Cause Analysis) services. Required if you use RCA features.
- `RCASERVICE_API_URL` – The full REST endpoint URL for RCA services. Set this to the RCA service address when enabling RCA integration.

> **Note:** If these environment variables are set, they will take precedence over values in your configuration files. This allows for flexible deployment and secure secret management.

---

### Setting Up Environment Variables

You can set environment variables in several ways depending on your operating system and development practices:

**Windows (Command Prompt):**
```cmd
set OPENAI_API_KEY=your_api_key
set OPENAI_API_BASE_ADDRESS=https://api.openai.com/v1
set OPENAI_MODEL=gpt-4
```

**Using .env Files for Local Development:**
Create a `.env` file in your project root:
```env
AI_API_KEY=your_api_key
AI_API_URL=https://api.openai.com/v1
AI_MODEL=gpt-4
```
Use tools like [DotNetEnv](https://github.com/tonerdo/dotnet-env) or [dotenv.net](https://github.com/bolorundurowb/dotenv.net) to automatically load these variables in your .NET application during development.

**Best Practices:**
- Avoid spaces around the `=` sign in .env files.
- Restart your terminal or IDE after setting environment variables.
- For production, set environment variables securely in your deployment environment.

### Example: Logging and Fault Analysis

```csharp
var logger = serviceProvider.GetRequiredService<ILogger<ApplicationLogger>>();
try
{
    // Your application logic
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred");
    // The logger will automatically trigger AI-powered fault analysis
}
```

---

## 🗂️ Project Structure

The solution is organized into several core libraries and folders, each with its own documentation:

- [src/Core/](src/Core/)  
- [src/Loggers/](src/Loggers/)  
- [tests/unittests/](tests/unittests/)  

<details>
  <summary>Expand for details</summary>

The solution is organized into several core libraries and folders, each with its own documentation:

- [src/Core/](src/Core/)  
  Core logic, domain models, configuration, extensions, and helpers.
  - [Caching](src/Core/Caching/README.md): In-memory and file-based caching services.
  - [Configuration](src/Core/Configuration/README.md): Strongly-typed settings, HTTP resilience, and OpenAI integration.
  - [Contracts](src/Core/Contracts/README.md): Interfaces and abstractions for logging, caching, AI, and more.
  - [Extensions](src/Core/Extensions/README.md): Extension methods for .NET types and solution utilities.
  - [Helpers](src/Core/Helpers/README.md): Utility classes for reflection, JSON, file system, error handling, and more.
  - [Models](src/Core/Models/README.md): Data models for analysis, logging, exceptions, chat, and more.
  - [Services](src/Core/Services/README.md): AI chat, embedding, and fault analysis services.

- [src/Loggers/](src/Loggers/)  
  Logging infrastructure, providers, publishers, and models.
  - [Application](src/Loggers/Application/README.md): Custom logger, logger factory, and provider for structured logging.
  - [Contracts](src/Loggers/Contracts/README.md): Publisher interface for log output.
  - [Extensions](src/Loggers/Extensions/README.md): DI and logging builder extensions for logger registration.
  - [Models](src/Loggers/Models/README.md): OpenTelemetry-compliant log event models.
  - [Publishers](src/Loggers/Publishers/README.md): Console and EventSource publishers for log output.

- [tests/unittests/](tests/unittests/)  
  Unit tests for all major components.

</details>

---

## ✨ Logging Infrastructure

AIEventing provides a flexible, extensible logging infrastructure:

- **OpenTelemetry-compliant log event serialization**
- **Customizable log event model via `ILogEvent`**
- **Asynchronous, thread-safe publishing with `ConsolePublisher`**
- **Integration with Microsoft.Extensions.Logging abstractions**
- **Pluggable publisher model via `IPublisher`**
- **EventSource-based logging for ETW and advanced diagnostics**
- **Testability with event counters**

### Registering the Logger

```csharp
// In your DI setup (e.g., Startup.cs or Program.cs)
services.InitializeServices(builder.Configuration);
services.InitializeLogging(builder.Configuration);
```

---

## 🧪 Testing

Run all unit tests:

```sh
dotnet test
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

For questions or support, please contact Dwaine Gilmer at [Protonmail.com](mailto:dwaine.gilmer@protonmail.com) or submit an issue on the project's GitHub
