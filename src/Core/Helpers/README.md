# 🧠 Helpers

This folder contains utility classes that provide common helper methods for reflection, JSON validation, file system operations, error creation, exception handling, caching, and regular expressions. These helpers are designed to simplify and standardize operations throughout the Core project.

---

## 📦 Classes

### 🔍 `ReflectionHelper`
Provides utility methods for reflection-based operations, such as determining whether a property value should be ignored during updates based on its type and value.

**Key Method:**
- `ShouldIgnoreProperty(PropertyInfo property, object? value)`: Returns `true` if the property should be ignored (e.g., null, empty string, default date, empty collection).

---

### 🧾 `JsonHelpers`
Offers methods for validating JSON strings against the structure of a specified type and for parsing or extracting JSON-related data.

**Key Methods:**
- `ValidateStrict<T>(string json)`: Ensures the JSON string contains only properties defined in type `T`.
- `ParseUrls(IList<string> input)`: Converts a list of URLs into a list of `ExternalReference` objects.
- `ExtractJson(string text)`: Extracts a JSON object from a string containing extra text.

---

### 📁 `FileSystemHelpers`
Provides utility methods for file system operations, such as generating safe file names and paths.

**Key Methods:**
- `FileSystemName(this string obj, int length = 64)`: Generates a sanitized file system name from a string.
- `GetFilePath(string fileName, string directory, bool isJson = true)`: Constructs a file path for a given file name and directory.
- `SanitizeForFileSystem(string input, FileSystemSanitizeOptions? options = null)`: Advanced sanitization for file system strings.

**Nested Class:**
- `FileSystemSanitizeOptions`: Configuration options for file system string sanitization.

---

### 💥 `ExceptionHelper`
Provides methods for generating unique hashes for exceptions and extracting inner exceptions.

**Key Methods:**
- `GetExceptionHash(SerializableException ex)`: Generates a SHA-256 hash for a serializable exception.
- `GetExceptionHash(Exception ex)`: Generates a SHA-256 hash for a standard exception.
- `GetExceptionHash(string input)`: Computes a SHA-256 hash for a string.
- `GetInnerExceptions(Exception? exception = null)`: Retrieves a list of inner exceptions as `SerializableException` objects.

---

### 🏭 `ErrorFactory`
Factory class for creating consistent error objects across different application layers.

**Key Methods:**
- `CreateWorkflowError(...)`: Creates a workflow-specific error.
- `CreateServiceError(...)`: Creates a service-specific error.
- `CreateApplicationError(...)`: Creates a general application error.
- `CreateValidationError(...)`: Creates a validation error.
- `CreateExceptionError(...)`: Creates an error from an exception.

---

### 🗝️ `CachingHelper`
Provides utility methods for generating cache keys.

**Key Method:**
- `GenCacheKey(string prepend, string key, string? hash = null)`: Generates a cache key by combining a prefix, key, and optional hash.

---

### 🧩 `CoreRegex`
Provides methods to sanitize JSON strings and extract URLs or domain names using precompiled regular expressions.

**Key Methods:**
- `SanitizeJson(string sanitized)`: Cleans and normalizes a JSON string.
- `ExtractUrl(string input)`: Extracts the first URL from a string.
- `ExtractDomainName(string input)`: Extracts the domain name from a string.

---

## 📝 Usage

These helpers are intended to be used throughout the Core project to simplify code and enforce best practices. For example:

```csharp
using Core.Helpers;
using System.Reflection;

// 🔍 ReflectionHelper
bool ignore = ReflectionHelper.ShouldIgnoreProperty(propertyInfo, value);

// 🧾 JsonHelpers
JsonHelpers.ValidateStrict<MyType>(jsonString);
var refs = JsonHelpers.ParseUrls(urlList);
string json = JsonHelpers.ExtractJson(text);

// 📁 FileSystemHelpers
string safeName = "My File Name".FileSystemName();
string path = FileSystemHelpers.GetFilePath("profile", "profiles");

// 💥 ExceptionHelper
string hash = ExceptionHelper.GetExceptionHash(exception);
var innerExceptions = ExceptionHelper.GetInnerExceptions(exception);

// 🏭 ErrorFactory
var error = ErrorFactory.CreateServiceError("MyService", "404", "Not found");

// 🗝️ CachingHelper
string cacheKey = CachingHelper.GenCacheKey("prefix", "key");

// 🧩 CoreRegex
string cleanJson = CoreRegex.SanitizeJson(rawJson);
string url = CoreRegex.ExtractUrl(text);
string domain = CoreRegex.ExtractDomainName(url);
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

For questions or support, please contact Dwaine Gilmer at [Protonmail.com](mailto:dwaine.gilmer@protonmail.com) or submit an issue on