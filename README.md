# FiloShop 🛍️

A production-ready e-commerce platform built with **.NET 9.0**, implementing **Domain-Driven Design (DDD)**, **CQRS**, and **Clean Architecture** principles.

## 🏗️ Architecture

FiloShop follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│         FiloShop.Api                    │  ← HTTP Entry Point
├─────────────────────────────────────────┤
│      FiloShop.Presentation              │  ← Controllers & DTOs
├─────────────────────────────────────────┤
│      FiloShop.Application               │  ← CQRS Handlers & Validation
├─────────────────────────────────────────┤
│         FiloShop.Domain                 │  ← Business Logic & Entities
├─────────────────────────────────────────┤
│  FiloShop.Infrastructure.Persistence    │  ← Database & Repositories
│  FiloShop.Infrastructure.Services       │  ← External Services
├─────────────────────────────────────────┤
│      FiloShop.SharedKernel             │  ← Reusable Patterns
└─────────────────────────────────────────┘
```

## 📦 Project Layers

| Layer | Purpose | Documentation |
|-------|---------|---------------|
| **SharedKernel** | Reusable DDD building blocks (CQRS, Idempotency, Result pattern) | [README](./FiloShop.SharedKernel/README.md) |
| **Domain** | Core business logic, aggregates, value objects, domain events | [README](./FiloShop.Domain/README.md) |
| **Application** | Use cases, commands, queries, validation | [README](./FiloShop.Application/README.md) |
| **Presentation** | Controllers, DTOs, API contracts | [README](./FiloShop.Presentation/README.md) |
| **Api** | HTTP pipeline, middleware, startup configuration | [README](./FiloShop.Api/README.md) |
| **Infrastructure.Persistence** | EF Core, repositories, database migrations | [README](./FiloShop.Infrastructure.Persistence/README.md) |
| **Infrastructure.Services** | Authentication (Keycloak), caching (Redis), background jobs | [README](./FiloShop.Infrastructure.Services/README.md) |

## ✨ Key Features

### 🎯 Domain-Driven Design
- **6 Aggregates**: User, Order, CatalogItem, Basket, CatalogBrand, CatalogType
- **Value Objects**: Money, Address, Email, etc.
- **Domain Events**: Async communication via Outbox pattern

### 🔁 CQRS & MediatR
- **Commands** for writes (with idempotency)
- **Queries** for reads (with caching)
- **Pipeline Behaviors**: Validation, logging, unit of work

### 🔒 Idempotency
- Prevents duplicate command execution
- Client-provided or auto-generated keys
- PostgreSQL-backed storage

### 📤 Outbox Pattern
- Reliable event publishing
- Transactional consistency
- Background processing with Quartz.NET

### 🔐 Authentication & Authorization
- **Keycloak** integration
- JWT Bearer tokens
- Permission-based access control

### 💾 Persistence
- **PostgreSQL** database
- **EF Core 9.0** with interceptors
- Automatic audit fields (`CreatedAt`, `UpdatedAt`)
- Comprehensive entity configurations

### 📊 Observability
- **Serilog** logging
- **Seq** log aggregation
- Structured logging throughout

### ⚡ Caching
- **Redis** for distributed caching
- Automatic query result caching
- Configurable expiration

## 🚀 Getting Started

### Prerequisites
- **.NET 9.0 SDK**
- **Docker Desktop** (for local development)

### Run with Docker Compose

```powershell
# Start all services
docker compose up --build

# Access the API
http://localhost:5000/swagger
```

### Services
- **API**: http://localhost:5000
- **PostgreSQL**: localhost:5433
- **Redis**: localhost:6380
- **Keycloak**: http://localhost:8080
- **Seq**: http://localhost:5341

### Run Migrations

```powershell
cd FiloShop.Api
dotnet ef migrations add MigrationName -p ../FiloShop.Infrastructure.Persistence -s .
dotnet ef database update -p ../FiloShop.Infrastructure.Persistence -s .
```

## 🧪 Testing

```powershell
# Unit Tests
dotnet test FiloShop.Domain.UnitTests

# Integration Tests
dotnet test FiloShop.IntegrationTests

# Architecture Tests
dotnet test FiloShop.ArchitectureTests

# Naming Tests
dotnet test FiloShop.NamingTests
```

## 📚 Documentation

Each layer has detailed documentation. Start here:

- **[SharedKernel](./FiloShop.SharedKernel/README.md)** - Core patterns and reusable components
- **[Domain](./FiloShop.Domain/README.md)** - Business logic and aggregates
- **[API](./FiloShop.Api/README.md)** - HTTP endpoints and middleware

## 📖 Technical Deep-Dive Guides

Comprehensive guides for advanced topics:

| Guide | Topic | Location       |
|-------|-------|----------------|
| **[Idempotency](./FiloShop.SharedKernel/readme_idempotency.md)** | Prevent duplicate command execution | SharedKernel   |
| **[Outbox Pattern](./FiloShop.SharedKernel/readme_outbox.md)** | Reliable event publishing | SharedKernel   |
| **[Domain Events](./FiloShop.SharedKernel/readme_domain_events.md)** | Event-driven architecture | Domain         |
| **[Migrations](./FiloShop.Infrastructure.Persistence/readme_migrations.md)** | Database schema evolution | Infrastructure |
| **[Entity Configurations](./FiloShop.Infrastructure.Persistence/readme_entity_configurations.md)** | EF Core Fluent API | Infrastructure |
| **[Testing Strategy](./readme_testing.md)** | Unit, integration & architecture tests | Root           |
| **[Keycloak Setup](./readme_keycloak_setup.md)** | Authentication & authorization | Root           |

## 🛠️ Tech Stack

### Core
- **.NET 9.0** - Runtime & SDK
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9.0** - ORM

### Patterns & Libraries
- **MediatR** - CQRS implementation
- **FluentValidation** - Request validation
- **Dapper** - High-performance data seeding

### Infrastructure
- **PostgreSQL 16** - Primary database
- **Redis 7** - Distributed cache
- **Keycloak** - Identity & access management
- **Seq** - Centralized logging
- **Quartz.NET** - Background job scheduling

### DevOps
- **Docker** & **Docker Compose**
- **Multi-stage Dockerfiles** for optimized builds

## 🎯 Design Principles

✅ **SOLID** principles  
✅ **DDD** tactical patterns  
✅ **Clean Architecture** with dependency inversion  
✅ **CQRS** for read/write separation  
✅ **Event-Driven** with domain events  
✅ **Persistence Ignorance** in domain layer  

## 📁 Project Structure

```
FiloShop/
├── FiloShop.Api/                    # HTTP Entry Point
├── FiloShop.Presentation/           # Controllers
├── FiloShop.Application/            # Use Cases
├── FiloShop.Domain/                 # Business Logic
├── FiloShop.Infrastructure.Persistence/  # Database
├── FiloShop.Infrastructure.Services/     # External Services
├── FiloShop.SharedKernel/           # Reusable Patterns
├── FiloShop.ArchitectureTests/      # Enforce Boundaries
├── FiloShop.Domain.UnitTests/       # Domain Tests
├── FiloShop.IntegrationTests/       # API Tests
├── FiloShop.NamingTests/            # Convention Tests
├── docker-compose.yaml              # Full Stack Orchestration
└── README.md                        # This File
```

## 🤝 Contributing

See [CONTRIBUTION.md](./CONTRIBUTION.md) for guidelines.

## 📄 License

This project is licensed under the MIT License. See [LICENSE.md](./LICENSE.md) for details.

---

**Built with ❤️ using Domain-Driven Design and Clean Architecture**