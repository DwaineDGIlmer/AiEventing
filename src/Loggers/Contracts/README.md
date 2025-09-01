# 🧠 Loggers.Contracts

This folder contains interfaces that define contracts for logging-related components, such as publishers that write log messages to various outputs.

---

## 📦 Classes

### `IPublisher`
Defines a contract for publishing messages to an output writer, such as a console, file, or network stream.

**Key Methods:**
- `Task WriteLine(string message)`: Writes a message to the output, followed by a line terminator (default is `\r\n`).
- `Task Write(string message)`: Writes a message to the output without appending a line terminator.

**Usage Example:**
```csharp
public sealed class ConsolePublisher : IPublisher
{
    public Task WriteLine(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }

    public Task Write(string message)
    {
        Console.Write(message);
        return Task.CompletedTask;
    }
}
```

Implementations of `IPublisher` are responsible for handling the actual output logic, making it easy to swap between different output mechanisms (console, file, remote logging, etc.) in your logging infrastructure.

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