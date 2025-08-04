# Services

This folder contains core service classes that provide integration with AI, embedding, and fault analysis functionality. These services are designed to encapsulate business logic for interacting with external APIs (such as OpenAI), managing embeddings, and analyzing faults using AI-driven or remote services.

---

## Classes

### `FaultAnalysisService`
Provides functionality for analyzing faults using AI services. This service integrates with external AI APIs (such as OpenAI or a custom RCA service) to analyze fault-related data, send fault descriptions and events for processing, and return analysis results.

**Key Features:**
- Sends fault context or log events to AI endpoints for analysis.
- Supports both OpenAI and custom RCA (Root Cause Analysis) service endpoints.
- Handles configuration for API keys, endpoints, and models.
- Logs diagnostic and operational information.

**Key Methods:**
- `Task<OpenAiChatResponse> AnalyzeFaultAsync(IList<OpenAiMessage> messages)`: Sends a collection of messages to the OpenAI API and returns the analysis result.
- `Task<bool> AnalyzeFaultAsync(ILogEvent fault)`: Sends a fault event to the RCA service for analysis and returns whether the analysis was successful.

**Usage Example:**
```csharp
var service = new FaultAnalysisService(httpClientFactory, openAiOptions, aiEventOptions);
var result = await service.AnalyzeFaultAsync(messages); // For OpenAI analysis
var success = await service.AnalyzeFaultAsync(logEvent); // For RCA service analysis
```

---

### `OpenAiChatService`
Provides services for interacting with OpenAI's API, including generating chat completions and embeddings. This class acts as a wrapper around the OpenAI client, enabling simplified access to OpenAI's chat and embedding functionality, and leverages caching for performance.

**Key Features:**
- Generates chat completions based on system and user messages.
- Uses caching to avoid redundant API calls.
- Exposes the underlying `OpenAIClient` and configuration.

**Key Methods:**
- `Task<T> GetChatCompletion<T>(string systemMessage, string userMessage)`: Sends system and user messages to OpenAI and returns a strongly-typed response.

**Usage Example:**
```csharp
var chatService = new OpenAiChatService(openAiClient, openAiOptions, cacheService, logger);
var summary = await chatService.GetChatCompletion<AnalysisSummary>("System prompt", "User question");
```

---

### `OpenAiEmbeddingService`
Provides functionality to generate text embeddings using the OpenAI API. This service communicates with the embedding API endpoint to generate embeddings for a given text input and uses caching to optimize repeated requests.

**Key Features:**
- Generates numerical embeddings for text using a specified OpenAI model.
- Validates embedding dimensions.
- Caches embeddings for repeated queries.

**Key Methods:**
- `Task<float[]> GetEmbeddingAsync(string text)`: Returns the embedding for the given text, using cache if available.

**Usage Example:**
```csharp
var embeddingService = new OpenAiEmbeddingService(openAiClient, cacheService);
float[] embedding = await embeddingService.GetEmbeddingAsync("Sample text to embed");
```

---

## Usage

These services are intended to be registered with dependency injection and used throughout the application for AI-driven analysis, chat, and embedding operations.

**Example: Registering and using services**
```csharp
// Registration (in Startup or Program)
services.AddSingleton<OpenAiChatService>();
services.AddSingleton<OpenAiEmbeddingService>();
services.AddSingleton<FaultAnalysisService>();

// Usage (in your application code)
var chatResult = await openAiChatService.GetChatCompletion<AnalysisSummary>("System", "User");
var embedding = await openAiEmbeddingService.GetEmbeddingAsync("Text to embed");
var analysis = await faultAnalysisService.AnalyzeFaultAsync(messages);
```

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

---

## License

This project is licensed under the MIT License.

---

## Contacts

For questions or support,