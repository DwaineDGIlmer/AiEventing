# Caching Layer

This folder contains caching-related services and abstractions for the Core project. The primary implementation provided is an in-memory cache service that can be used throughout the solution.

---

## Features

- **MemoryCacheService**: Implements `ICacheService` using `IMemoryCache` for efficient in-memory caching.
  - Supports asynchronous methods for retrieving, storing, and removing cached items.
  - Caching can be enabled or disabled via constructor parameter.
  - Allows setting absolute expiration for cache entries.
- **FileCaching**: Provides file-based caching for scenarios where persistence across application restarts is required.
  - Stores cache entries as files on disk.
  - Supports asynchronous methods for reading, writing, and removing cached items.
  - Allows setting expiration for cache files.
  - Useful for large or persistent cache scenarios where in-memory caching is insufficient.

---

## Usage

```csharp
using Core.Caching;
using Microsoft.Extensions.Caching.Memory;

var memoryCache = new MemoryCache(new MemoryCacheOptions());
var cacheService = new MemoryCacheService(memoryCache, enabled: true);

// Store a value
await cacheService.CreateEntryAsync("myKey", "myValue", TimeSpan.FromMinutes(10));

// Retrieve a value
var value = await cacheService.TryGetAsync<string>("myKey");

// Remove a value
await cacheService.RemoveAsync("myKey");
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