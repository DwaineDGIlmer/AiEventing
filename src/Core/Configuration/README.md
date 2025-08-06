# 🧠 Configuration Module	
This module provides a configuration management system for the application, allowing for easy access and modification of settings.

## ✨ Features
Allows configurations to be loaded from various sources, such as JSON files, environment variables, or other configuration providers.

### AiEventSettings
This class is considered the main configuration class for the Core. It contains properties that can be set to configure the behavior of the AI event system.

### Usage
To use the `AiEventSettings`, you can inject the `IConfigurationService` into your classes and access the settings as needed.
```csharp
public static IServiceCollection ConfigureAiEventSettings(this IServiceCollection services, IConfiguration configuration)
{
    services.IsNullThrow();
    configuration.IsNullThrow();

    // Bind the AiEventSettings from configuration
    services.Configure<OpenAiSettings>(options =>
    {
        // Bind configuration values to options
        configuration.GetSection(nameof(AiEventSettings)).Bind(options);

        // Apply environment variable and default overrides
        if (options.IsEnabled)
        {
            options.BaseAddress = string.IsNullOrEmpty(options.BaseAddress) ? Defaults.OpenAiABaseAddress : options.BaseAddress;
            options.Endpoint = string.IsNullOrEmpty(options.Endpoint) ? Defaults.OpenAiEndpoint : options.Endpoint;
            options.ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? options.ApiKey ?? string.Empty;
            options.Model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? options.Model;
        }
    });
}
```

---

### BulkheadSettings 
Supports resilient HTTP clients and configures the bulkhead pattern for limiting concurrent requests. This class defines settings for the bulkhead pattern, which is used to limit the number of concurrent requests to a service.

**Usage Example:**
```csharp
var bulkheadSettings = new BulkheadSettings
{
    Enabled = true,
    MaxParallelization = 10,
    MaxQueuingActions = 20
};
```

---

### CircuitBreakerSettings
Supports resilient HTTP clients and configures the circuit breaker pattern for handling failures. This class defines settings for the circuit breaker pattern, which is used to prevent cascading failures in a distributed system.

**Usage Example:**
```csharp
var circuitBreakerSettings = new CircuitBreakerSettings
{
    Enabled = true,
    DurationOfBreak = 30, // seconds
    FailureThreshold = 5
};
```

---

### OpenAiSettings
Contains settings specific to OpenAI integration, such as API keys, model, and endpoint configurations.

**Usage Example:**
```csharp
var openAiSettings = new OpenAiSettings
{
    ApiKey = "your-openai-api-key",
    BaseAddress = "https://api.openai.com/",
    Endpoint = "/v1/completions",
    Model = "gpt-4",
    ClearCache = false
};
```

---

### ResilientHttpPolicySettings
Defines settings for resilient HTTP policies, including retry and timeout configurations for HTTP requests.

**Usage Example:**
```csharp
var policy = new ResilientHttpPolicy
{
    HttpClientName = "MyHttpClient",
    PolicyName = "DefaultPolicy",
    Enabled = true,
    PolicyOrder = 0,
    HttpTimeout = 30,
    UseStandardResilience = true,
    RetryPolicy = new RetrySettings { Enabled = true, MaxRetryCount = 3 },
    CircuitBreakerPolicy = new CircuitBreakerSettings { Enabled = true, DurationOfBreak = 30, FailureThreshold = 5 },
    BulkheadPolicy = new BulkheadSettings { Enabled = true, MaxParallelization = 10, MaxQueuingActions = 20 }
};
```

---

### RetrySettings
Defines settings for retry policies, including the number of retries and delay between retries.

**Usage Example:**
```csharp
var retrySettings = new RetrySettings
{
    Enabled = true,
    MaxRetryCount = 3,
    Delay = 1,      // seconds
    MaxDelay = 10,  // seconds
    Jitter = 5      // seconds
};
```

---

### SerpApiSettings
Contains settings for integrating with the SerpApi service, including API keys and endpoint configurations. 

#### Usage
To use the configuration system, you can inject the `IConfigurationService` into your classes and access the settings as needed.
**Usage Example:**
```csharp
var serpApiSettings = new SerpApiSettings
{
    CacheExpirationInMinutes = 60, //   cache expiration time in minutes
    Query = "Data Engineer",    // query used for SerpApi request
    Location = "Charlotte, NC", // location used for SerpApi request
};
```


## 🤝 Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.  
See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

---

## 📄 License

This project is licensed under the [MIT License](../../LICENSE).

---

## 📬 Contact

For questions or support, please contact Dwaine Gilmer at [Protonmail.com](mailto:dwaine.gilmer@protonmail.com) or submit an issue on the project's GitHub
