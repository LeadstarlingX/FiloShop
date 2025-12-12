# FiloShop 🛍️

A learning-focused, "nearly" production-ready e-commerce platform built with **.NET 9**, implementing **DDD**, **CQRS**, and **Clean Architecture**.

> **Status**: 🚧 Work in Progress (Documentation & Structure Phase), more features to add. :(

## 🏗️ Architecture
The system uses a **Layered Architecture** with strict dependency rules:

| Layer | Responsibility | Key Design Decisions |
|-------|----------------|----------------------|
| **[Api](./FiloShop.Api/README.md)** | HTTP Entry Point | Thin translation layer, no business logic. |
| **[Presentation](./FiloShop.Presentation/README.md)** | Controllers/DTOs | Separated to allow swapping hosting models. |
| **[Application](./FiloShop.Application/README.md)** | Use Cases (CQRS) | Orchestrates domain; uses MediatR behaviors. |
| **[Domain](./FiloShop.Domain/README.md)** | Business Logic | **Rich Domain Model**. Pure C#, no dependencies. |
| **[Infrastructure](./FiloShop.Infrastructure.Persistence/README.md)** | Persistence | EF Core + Dapper. Only layer knowing SQL. |
| **[SharedKernel](./FiloShop.SharedKernel/README.md)** | Building Blocks | "Batteries-included" base classes & patterns. |

## 🚀 Getting Started

### Prerequisites
- **Docker Desktop** (Required)
- **.NET 9.0 SDK** (Optional, for fast local dev)

### 🐳 Run with Docker (Recommended)
This spins up the entire stack: API, Postgres, Redis, Keycloak, Seq.

```powershell
docker compose up -d --build
```

### 🔗 Service Endpoints
| Service | Local URL | Internal Docker Port |
|---------|-----------|----------------------|
| **Swagger API** | [http://localhost:8000/swagger](http://localhost:8000/swagger) | 8080 |
| **PostgreSQL** | `localhost:5433` | 5432 |
| **Redis** | `localhost:6379` | 6379 |
| **Keycloak** | [http://localhost:8001](http://localhost:8001) | 8080 |
| **Seq Logs** | [http://localhost:8081](http://localhost:8081) | 80 |

### 🛠️ Manual Setup Notes
1.  **Migrations**: Applied automatically on startup. No action needed.
2.  **Keycloak**: Requires manual realm setup. See [Keycloak Setup Guide](./readme_keycloak_setup.md).
3.  **Tests**: See [Testing Guide](./readme_testing.md).

## 📚 Technical Deep Dives
- **[SharedKernel Architecture](./FiloShop.SharedKernel/README.md)** - The "Why" behind the patterns.
- **[Domain Events](./FiloShop.Domain/readme_domain_events.md)** - Strict rules for side effects.
- **[Entity Configuration](./FiloShop.Infrastructure.Persistence/readme_entity_configurations.md)** - EF Core guidelines.
- **[Testing Strategy](./readme_testing.md)** - TUnit & Testcontainers setup.

## 🤝 Contributing
Please read the [CONTRIBUTION.md](./CONTRIBUTION.md) carefully. We enforce strict architectural boundaries.