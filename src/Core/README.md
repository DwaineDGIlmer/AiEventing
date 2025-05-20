# Core

The **Core** project contains the fundamental building blocks and reusable code for the AIEventing solution. It is designed to be independent of other layers and provides shared logic, domain models, and abstractions that can be referenced by other assemblies.

## Purpose

- Houses domain entities, value objects, and core business logic.
- Provides interfaces and abstractions for use by other layers (such as Application and Infrastructure).
- Contains reusable utilities and helper classes that are not dependent on external frameworks.

## Guidelines

- Keep this project free of dependencies on other solution layers or external technologies.
- Only include code that is intended to be shared and reused across multiple projects.
- Maintain a clear separation of concerns and avoid implementation details specific to other layers.

## Typical Contents

- Domain models and entities
- Value objects
- Interfaces and abstractions
- Shared constants and enums
- Utility and helper classes

## References

- Other projects (Application, Infrastructure, etc.) may reference Core, but Core should not reference them.

---