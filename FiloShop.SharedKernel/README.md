# FiloShop.SharedKernel

The **SharedKernel** is a reusable foundational layer containing common building blocks for Domain-Driven Design (DDD) and Clean Architecture. It provides base classes, interfaces, and patterns used across all projects.

## 📦 What's Inside

### 🎯 CQRS (Command Query Responsibility Segregation)

Complete implementation of the CQRS pattern using MediatR:

- **Commands**: `ICommand`, `ICommand<TResponse>`, `ICommandHandler<>`
- **Queries**: `IQuery<TResponse>`, `IQueryHandler<>`
- **Idempotent Commands**: `IIdempotentRequest<TResponse>` with automatic deduplication

### 🔁 Idempotency System

Prevents duplicate command execution with persistence-backed tracking:

- `IIdempotencyStore` - Interface for storage
- `IdempotencyRecord` - Stores executed command results
- `IdempotentCommandBehavior` - MediatR pipeline behavior
- See [readme_idempotency.md](./readme_idempotency.md) for detailed documentation

### 🧩 Base Entities & Events

- `BaseEntity` - Base aggregate root with:
  - Auto-managed `CreatedAt`/`UpdatedAt` (via interceptor)
  - Domain event support
  - Guid-based identity
- `IDomainEvent` - Marker interface for domain events
- Automatic audit field population via `AuditableEntityInterceptor`

### 🔧 MediatR Behaviors (Pipeline)

Cross-cutting concerns implemented as behaviors:

1. **ValidationBehavior** - FluentValidation integration
2. **LoggingBehavior** - Request/response logging
3. **QueryCachingBehavior** - Automatic query result caching
4. **IdempotentCommandBehavior** - Command deduplication
5. **UnitOfWorkBehavior** - Transaction management

### 🌐 API Standards

- `ApiResponse<T>` - Standardized API response wrapper
- `PaginatedResult<T>` - Pagination support
- `PagedQuery` - Base class for paginated queries

### 📤 Outbox Pattern

Reliable event publishing for distributed systems:

- `OutboxMessage` - Event storage for transactional outbox
- Ensures domain events are eventually published

### 🛠️ Core Interfaces

- `IUnitOfWork` - Transaction boundary
- `ISqlConnectionFactory` - Database connection factory
- `ICacheService` - Caching abstraction
- `IDateTimeProvider` - Testable time provider

### ⚙️ Providers

- `SystemDateTimeProvider` - Production time provider
- Supports dependency injection for testability

### ❌ Result Pattern

Type-safe error handling without exceptions:

- `Result` / `Result<T>` - Success/failure representation
- `Error` - Structured error information
- Eliminates throw/catch for business logic failures

## 🔗 Dependencies

- **MediatR** - CQRS and pipeline behaviors
- **FluentValidation** - Request validation
- **Microsoft.Extensions.Caching.Abstractions** - Caching

## 📚 Usage Examples

### Command with Idempotency
```csharp
public record CreateOrderCommand(Guid UserId, Address ShippingAddress) 
    : IIdempotentRequest<Guid>
{
    public Guid IdempotencyKey { get; init; }
}
```

### Query with Caching
```csharp
public record GetUserByIdQuery(Guid UserId) : ICachedQuery<UserDto>
{
    public string CacheKey => $"user-{UserId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
```

### Domain Event
```csharp
public record OrderCreatedDomainEvent(Guid OrderId) : IDomainEvent;

// In entity
var order = new Order(...);
order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));
```

## 🎯 Design Principles

- **Dependency-Free**: No references to infrastructure or domain-specific logic
- **Reusable**: Can be used across multiple projects
- **Testable**: All abstractions support mocking
- **DDD-Aligned**: Implements tactical DDD patterns

## 🚀 Getting Started

This layer is automatically referenced by Domain and Infrastructure layers. No manual setup required.

For idempotency implementation details, see [readme_idempotency.md](./readme_idempotency.md).