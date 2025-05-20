# AIEventing

AIEventing is a .NET project that provides core utilities and extension methods for event-driven and AI-enabled applications. It leverages dependency injection, configuration, and logging using Microsoft.Extensions libraries, and offers convenient JSON serialization helpers and initialization patterns.

## Features

- **Dependency Injection**: Easily register and resolve services using Microsoft.Extensions.DependencyInjection.
- **Configurable Logging**: Integrate console logging with flexible configuration.
- **JSON Serialization Extensions**: Serialize and deserialize objects with customizable `JsonSerializerOptions`.
- **Initialization Enforcement**: Ensure components are properly initialized before use.
- **Null and Empty Checks**: Extension methods for checking null or empty values on objects and strings.

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or later

### Building the Project

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/AIEventing.git
    cd AIEventing
    ```

2. Build the solution:
    ```sh
    dotnet build
    ```

### Running the Console Application

Navigate to the `src\ConsoleApp` directory and run:

```sh
dotnet run --project src\ConsoleApp
```
## Usage

### Initialization

To initialize the core components and configure JSON serialization, call `InitializeServices` during service registration in your application:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register your services here
        services.AddTransient<MyService>();
        // Initialize AIEventing core services and JSON options
        services.InitializeServices(context.Configuration);
    })
    .Build();
    
```

This ensures that all required settings and options are configured before using extension methods like `ToJson()` or `FromJson()`.

---


## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.

## License

This project is licensed under the MIT License.

---

**Note:**  
If you encounter issues with Windows Security blocking the build or debug process, add your project folder to Windows Security exclusions.
