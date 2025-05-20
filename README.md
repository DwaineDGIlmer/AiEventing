# AIEventing

AIEventing is a .NET solution for advanced, AI-assisted event logging and fault analysis. It provides structured logging, resilient HTTP clients, and integration with AI models (such as GPT-4) to analyze exceptions and stack traces, offering actionable insights for developers.

---

## Features

- **Structured Logging:**  
  Customizable, scoped, and structured logging with support for multiple publishers.

- **AI-Powered Fault Analysis:**  
  Integrates with AI models (e.g., GPT-4) to analyze .NET stack traces and suggest fixes.

- **Resilient HTTP Clients:**  
  Uses Polly for retry, circuit breaker, and bulkhead policies to ensure robust communication with external services.

- **Extensible Architecture:**  
  Easily add new log publishers, fault analysis providers, or serialization strategies.

---

## Getting Started

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

## Usage

### Configuration

Set the following environment variables or update your configuration:

- `AI_API_KEY` – Your AI model API key
- `AI_API_URL` – The endpoint for the AI model (e.g., OpenAI API)
- `AI_MODEL` – The model name (e.g., `gpt-4`)

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

## Project Structure

- `src/Core/` – Core logic, extensions, and configuration
- `src/Loggers/` – Logging infrastructure and publishers
- `src/Core/Services/` – Fault analysis and AI integration
- `tests/unittests/` – Unit tests for all major components

---

## Testing

Run all unit tests:

```sh
dotnet test
```

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements or bug fixes.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

## Acknowledgements

- [Polly](https://github.com/App-vNext/Polly) for resilience and transient-fault-handling
- [OpenAI](https://openai.com/) for AI model integration