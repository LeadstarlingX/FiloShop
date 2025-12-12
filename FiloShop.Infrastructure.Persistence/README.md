# FiloShop.Infrastructure.Persistence

## 🏗️ Architecture
This layer implements all data access using **Entity Framework Core 9**. It is the *only* layer that knows about the database schema.

**Key Design Decisions:**
- **Zero-Repository Pattern**: We use generic `IRepository<T>` (or specific ones if needed) backed by standard EF Core `DbSet`.
- **Interceptors over Triggers**: We use `AuditableEntityInterceptor` C# logic to handle `CreatedAt`/`UpdatedAt` instead of SQL triggers, keeping logic testable and in code.
- **Fluent API Only**: database mapping is done strictly via `IEntityTypeConfiguration` classes, keeping the Domain clean of attributes.

## 🛠️ Capabilities

### 1. Migrations
Migrations are applied **automatically** at application startup. You do not need to run `dotnet ef database update` manually for local development.

**Migration Workflow**:
```powershell
# Add a migration (from solution root)
dotnet ef migrations add [Name] -p FiloShop.Infrastructure.Persistence -s FiloShop.Api
```

### 2. Database Seeding
The system automatically seeds initial data (Admin User, Catalog Types) if the database is empty. See `DataSeeder.cs`.

### 3. Interceptors
- `AuditableEntityInterceptor`: Automatically sets `CreatedAt` on Insert and `UpdatedAt` on Update.
- `DispatchDomainEventsInterceptor`: (Planned) Automates event dispatching after transaction commit.

### 4. Query Handlers (Dapper)
This layer implements all `IQueryHandler<TQuery, TResult>` interfaces defined in Application.
- Uses **Dapper** for high-performance reads.
- Bypasses Domain Entities (returns DTOs/Records directly).
- Located in `QueryHandlers/[Controller]/[Feature]` folders.

```
FiloShop.Infrastructure.Persistence/
├── QueryHandlers/
│   ├── Users/
│   │   └── GetUserByIdQueryHandler.cs // Implements IQueryHandler using Dapper
│   ├── Orders/
```

## 📚 Deep Dive Guides
- [Entity Configuration Guide](./readme_entity_configurations.md)
- [Migrations Guide](./readme_migrations.md)