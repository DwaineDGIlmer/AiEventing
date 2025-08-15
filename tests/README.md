# 🛠️ Unit Tests
[![Unit Tests](https://img.shields.io/github/actions/workflow/status/your-org/IntelligentLogging/dotnet-test.yml?branch=main)](https://github.com/your-org/IntelligentLogging/actions/workflows/dotnet-test.yml)
[![Build Status](https://img.shields.io/github/actions/workflow/status/your-org/IntelligentLogging/dotnet.yml?branch=main)](https://github.com/your-org/IntelligentLogging/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Deployed App](https://img.shields.io/badge/Azure-Live-blue)](https://intelligentlogging-fcgtc5gfazcaaeej.centralus-01.azurewebsites.net/)

---

## 📚 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Solution Structure](#️-solution-structure)
- [Getting Started](#-getting-started)
- [Running Tests](#-running-tests)
- [Test Coverage](#-test-coverage)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🚀 Overview

Unit tests in the AiEventing solution are designed to ensure the reliability, correctness, and maintainability of all major components. The primary goals of these unit tests are:

- **Validate Core Logic:** Ensure that business logic, data processing, and event handling behave as expected under various scenarios.
- **Prevent Regressions:** Catch bugs early by automatically testing new changes and protecting against unintended side effects.
- **Support Refactoring:** Enable safe code refactoring by providing a safety net of automated tests.
- **Document Behavior:** Serve as executable documentation for how components are expected to function.
- **Facilitate Collaboration:** Make it easier for contributors to understand, extend, and verify the codebase.
- **Promote Best Practices:** Encourage modular, testable code through the use of dependency injection and mocking.

Tests are organized by feature and layer (Core, Logger, HTTP Harness, Benchmarks) and leverage [xUnit](https://xunit.net/) for test structure, [Moq](https://github.com/moq/moq4) for mocking dependencies, and [FluentAssertions](https://fluentassertions.com/) for expressive assertions. Coverage is measured with [coverlet](https://github.com/coverlet-coverage/coverlet) and reported in CI to maintain high coverage standards.

---

## ✨ Features

- Automated test execution via GitHub Actions
- Coverage collection with [coverlet](https://github.com/coverlet-coverage/coverlet)
- Mocking with Moq
- Fluent assertions for readable test code
- Organized by feature and layer (Controllers, Models, etc.)

---

## 🗂️ Solution Structure

```
AiEventing/
├── licenses/
└── tests/
    └── Core.UnitTests/
    └── HttpHarness/
    └── Logger.UnitTests/
    └── LoggerBenchMarkTests/
```

### Core.UnitTests

Contains unit tests for the core business logic of the AiEventing solution, including event processing, data handling, and integration with external services.

### HttpHarness
Contains tests for the HTTP harness that simulates API requests and responses, ensuring that the system behaves correctly under various conditions.

### Logger.UnitTests
Contains unit tests for the logging framework, verifying that logs are generated correctly, formatted properly, and contain the expected information.

### LoggerBenchMarkTests
Contains benchmark tests for the logging framework, measuring performance and efficiency under different load conditions.

---

## 🏁 Getting Started

1. **Restore dependencies:**
   ```sh
   dotnet restore
   ```
2. **Build the solution:**
   ```sh
   dotnet build
   ```

---

## 🧪 Running Tests

Run all unit tests from the solution root:

```sh
dotnet test --verbosity normal
```

To run tests with coverage:

```sh
dotnet test /p:CollectCoverage=true
```

---

## 📊 Test Coverage

Test coverage is collected using coverlet and reported in CI.  
To generate a local coverage report:

```sh
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

Coverage results are available in the `TestResults` directory.

---

## 🤝 Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements and bug fixes.  
See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

---

## 📄 License

This project is licensed under the [MIT License](../../LICENSE).

---

## 📬 Contact

For questions or support, please contact Dwaine Gilmer at [Protonmail.com](mailto:dwaine.gilmer@protonmail.com) or submit an issue on the project's GitHub repository.
