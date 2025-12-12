# FiloShop.SharedKernel

## 🏗️ Architecture & Design
The **SharedKernel** is the foundational "batteries-included" layer for FiloShop. It provides a standardized set of DDD patterns and cross-cutting concerns to allow specific domains to focus on business logic rather than plumbing.

**Key Design Decision**: This layer is deliberately *opinionated*. It includes core implementations for common patterns (MediatR, Specifications, Logging) to ensure consistency across the application, even if it trades some purely theoretical decoupling for practical developer velocity.

## 🔘 Patterns & Capabilities
The following architectural patterns are available "out of the box":

- **CQRS**: Clear separation of command (write) and query (read) responsibilities.
- **Result Pattern**: explicit, type-safe error handling to eliminate "exception-driven logic".
- **Transactional Outbox**: Guarantees that domain events are published reliably, even if the broker is temporarily down.
- **Idempotency**: Prevents duplicate processing of sensitive operations (like payments) when retries occur.
- **Pipeline Behaviors**: Automatic handling of cross-cutting concerns (Logging, Validation, Caching, Transaction Management) for every request.
- **DDD Building Blocks**: Base classes for Aggregate Roots, Entities, and Value Objects to enforce domain modeling rules.
- **Specifications**: Encapsulates complex query logic into reusable classes, keeping repositories simple.

## ⚖️ Architectural Decisions & Trade-offs

| Decision | Context & Benefit | Trade-off / Difficulty |
|----------|-------------------|------------------------|
| **Infrastructure inside SharedKernel** | Included Specification & Serilog libraries to create a "complete" starter kit for any DDD project. | Increases coupling; swapping logging/persistence libraries requires changes to this core layer. |
| **Result Pattern over Exceptions** | Forces developers to handle failure cases explicitly, improving system reliability. | Adds verbosity to method signatures and requires "unwrapping" results in the API layer. |
| **Automatic Pipeline Behaviors** | Validations and logging happen automatically, reducing code duplication in handlers. | Can obscure execution flow; developers must be aware of the "hidden" logic running around their handlers. |

## 📚 Deep Dive Guides
For implementation specifics of complex patterns, see:
- [Idempotency Deep Dive](./readme_idempotency.md)
- [Outbox Pattern Guide](./readme_outbox.md)