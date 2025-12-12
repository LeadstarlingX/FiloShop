# FiloShop.Application

## 🏗️ Architecture
This layer implements the **Use Cases** of the system using the **CQRS** pattern. It orchestrates domain logic but does not contain business rules itself.

**Key Responsibilities**:
- **Commands**: Handle side effects (Create, Update, Delete). Returns `Result<T>`.
- **Queries**: Handle reads. Implementation varies (EF Core or Dapper), but contract is defined here.
- **Orchestration**: Loads aggregates, invokes domain methods, and saves changes via Unit of Work.

## 📂 Structure

We organize by **Feature** (Vertical Slices).

> [!NOTE]
> **Query Handlers** are NOT implemented here. They reside in `Infrastructure.Persistence` because they use Dapper directly. This layer only defines the `Query` contract and `QueryValidator`.

```
FiloShop.Application/
├── Users/                  # Feature Slice
│   ├── Commands/
│   │   ├── RegisterUser/
│   │   │   ├── RegisterUserCommand.cs
│   │   │   ├── RegisterUserCommandHandler.cs
│   │   │   └── RegisterUserCommandValidator.cs
│   └── Queries/
│       └── GetUserById/
│           ├── GetUserByIdQuery.cs          # Contract
│           └── GetUserByIdQueryValidator.cs # Input Validation
```

## ⚙️ Request Pipeline (Behaviors)
Every request automatically passes through these middleware steps (in order):

1.  **Logging**: Logs request entry/exit and performance.
2.  **Idempotency**: (Commands only) Checks if request ID was already processed.
3.  **Caching**: (Queries only) Checks distributed cache for existing result.
4.  **Validation**: Validates inputs using FluentValidation.
5.  **UnitOfWork**: (Commands only) Commits transaction *after* handler success.

## 📝 How to Add a Feature
1.  **Define Command/Query**: Create the record with input data.
2.  **Implement Handler**: Write the logic to load entities and call methods.
3.  **Add Validator**: Create `AbstractValidator<T>` for the command.
4.  **Register**: MediatR automatically finds and registers all handlers/validators.