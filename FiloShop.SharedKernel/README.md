# FiloShop.SharedKernel

The **SharedKernel** layer contains core utilities, abstractions, and cross-cutting concerns used throughout the FiloShop solution.

### Key Directories and Components

- **Api/**: Common API primitives and utilities shared between layers.
- **Behaviors/**: Base behavioral patterns and reusable logic, including pipelines.
- **CQRS/**: Core objects, interfaces, and helpers for Command Query Responsibility Segregation.
- **Entities/**: Fundamental entity abstractions used system-wide.
- **Errors/**: Shared error handling types, exceptions, and result patterns.
- **Events/**: Domain event patterns and base classes.
- **Exceptions/**: Centralized exception types for consistent error handling.
- **Idempotency/**: Implementations and patterns for idempotent command processing.  
  See [readme_idempotency.md](./readme_idempotency.md) for deep dive.
- **Interfaces/**: Shared interfaces for contracts and service implementations.
- **Providers/**: Base provider interfaces (e.g. for services, strategies).
- **Results/**: Typed result objects for standardized outcomes and error signaling.

### Purpose

This layer's purpose is to maximize reusability for patterns, types, and core logic. It enables consistency and best practices across all system layers.

_This SharedKernel is continuously evolving and already includes foundational elements ready for use in all solution layers._

---

For details on Idempotency, see: [readme_idempotency.md](./readme_idempotency.md)