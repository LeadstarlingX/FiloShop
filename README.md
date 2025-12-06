# FiloShop

FiloShop is a layered solution adopting a clean architecture approach. Each major folder represents a layer or concern in the system.

## Project Layers

| Layer                                        | Description                                                                              |
|-----------------------------------------------|------------------------------------------------------------------------------------------|
| [FiloShop.Api](./FiloShop.Api/README.md)                              | API endpoints and entry points into the solution                                          |
| [FiloShop.Application](./FiloShop.Application/README.md)              | Application business logic and orchestration                                              |
| [FiloShop.ArchitectureTests](./FiloShop.ArchitectureTests/README.md)  | Tests for enforcing architectural boundaries                                              |
| [FiloShop.Domain](./FiloShop.Domain/README.md)                        | Core business entities and logic                                                          |
| [FiloShop.Domain.UnitTests](./FiloShop.Domain.UnitTests/README.md)    | Unit tests for the domain layer                                                           |
| [FiloShop.Infrastructure.Persistence](./FiloShop.Infrastructure.Persistence/README.md) | Data persistence, repositories, and database concerns                                     |
| [FiloShop.Infrastructure.Services](./FiloShop.Infrastructure.Services/README.md) | Infrastructure services and integrations                                                  |
| [FiloShop.IntegrationTests](./FiloShop.IntegrationTests/README.md)    | End-to-end and integration tests                                                          |
| [FiloShop.NamingTests](./FiloShop.NamingTests/README.md)              | Tests for naming conventions and guidelines                                               |
| [FiloShop.Presentation](./FiloShop.Presentation/README.md)            | UI or presentation layer                                                                  |
| [FiloShop.SharedKernel](./FiloShop.SharedKernel/README.md)            | Foundational utilities and cross-cutting abstractions ready for use across all layers     |

The [SharedKernel](./FiloShop.SharedKernel/README.md) layer already houses crucial, reusable components and patterns, and acts as a foundation for consistent best practices across the solution.

Each layer has its own [README.md](./README.md) with further details as the implementation progresses.

---

## Get Involved

Please see [CONTRIBUTION.md](./CONTRIBUTION.md) to learn how to contribute.

## License

This project is licensed under the MIT License. See [LICENSE.md](./LICENSE.md) for details.