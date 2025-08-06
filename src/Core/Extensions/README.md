# 🧠 Extensions

These classes contain extension methods and helper classes that provide utility functions for the Core project. These extensions are designed to simplify common operations, enhance code readability, and promote code reuse throughout the solution.

---

## 📦 Classes

### `Extensions`
A static class providing a variety of extension methods for general utility operations, including:

- **Hashing:**  
  - `GenHash<T>(this T obj)`: Generates a SHA256-based hash for an object and returns a `ulong`.
  - `GenHashString<T>(this T obj)`: Generates a SHA256-based hash for an object and returns a hex string.

- **HTTP Client Helpers:**  
  - `PostAsJsonAsync<T>(...)`: Sends a JSON-encoded object via HTTP POST using a named client.
  - `GetStringAsync(...)`: Sends a GET request and retrieves the response as a string.
  - `GetAsync(...)`: Sends a GET request and retrieves the response as an `HttpResponseMessage`.

- **Serialization:**  
  - `ToJson<T>(this T obj)`: Serializes an object to a JSON string.
  - `ToJsonTry<T>(this T obj)`: Tries to serialize an object to JSON, returning an empty string on failure.
  - `ToObject<T>(this string obj)`: Deserializes a JSON string to an object.
  - `ToObjectTry<T>(this string obj)`: Tries to deserialize a JSON string to an object, returning default on failure.

- **Null and Argument Checking:**  
  - `IsNull<T>(this T obj)`: Checks if an object is null (or empty if string).
  - `IsNullThrow<T>(this T obj, ...)`: Throws if the object is null or empty.
  - `IsNullOrEmpty<T>(this T obj)`: Checks if an object is null or, if string, empty.
  - `IsNotNull<T>(this T obj)`: Checks if an object is not null (or not empty if string).
  - `IsNullThrow<T>(this IList<T> list, ...)`: Throws if a list is null or empty.

---

### `DictionaryExtensions`
A static class providing extension methods for dictionaries.

- **RemoveNullValues:**  
  - `RemoveNullValues(this Dictionary<string, object?> dict)`: Removes all entries with `null` values from the dictionary.

---

### `ServiceCollectionExtensions`
A static class providing extension methods for configuring and initializing services in an `IServiceCollection`. Key features include:

- **Service Initialization:**  
  - `InitializeServices`: Configures and initializes services, binds settings from configuration, and sets up JSON serializer options.
- **Resilient HTTP Clients:**  
  - `AddResilientHttpClient`: Adds HTTP clients with Polly-based retry, circuit breaker, and bulkhead policies.
  - `AddBasicResilienceHandler`: Adds a standard resilience handler with timeout and circuit breaker strategies.
- **Settings and Policy Retrieval:**  
  - `GetAiEventSettings`, `GetOpenAiSettings`, `GetResilientHttpPolicy`, `GetSettings<T>`, `GetResilencyPolicies`: Retrieve and bind configuration settings and policies.
- **Policy Factories:**  
  - `GetBulkheadPolicy`, `GetRetryPolicy`, `GetCircuitBreakerPolicy`: Create Polly policies for HTTP resilience.
- **Service Registration Helpers:**  
  - `AddService<T>`: Adds a singleton service using a factory function.
- **File Caching:**  
  - `GetFileCaching`: Helper for adding file-based caching services.

---

## 📝 Usage

These extension methods are intended to be used throughout the Core project to simplify code and enforce best practices. For example:

```csharp
using Core.Extensions;

// Hashing an object
ulong hash = myObject.GenHash();

// Serializing to JSON
string json = myObject.ToJson();

// Null checking
myObject.IsNullThrow("Object must not be null.");

// Removing nulls from a dictionary
var cleanedDict = myDict.RemoveNullValues();
```

You can also use `ServiceCollectionExtensions` to configure services and HTTP clients with resilience policies in your application's startup:

```csharp
services.InitializeServices(configuration);
services.AddResilientHttpClient(configuration, "MyHttpClient");
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
