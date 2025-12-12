# Testing Strategy Guide

We use **TUnit** for all testing and **Testcontainers** for integration tests.

> [IMPORTANT !!]
> TUnit tests are executables. You run them directly, not via `dotnet test` (although `dotnet test` will work if configured).

## 🧪 Test Types

### 1. Unit Tests (`FiloShop.Domain.UnitTests`)
- **Scope**: Domain logic (Aggregates, Value Objects).
- **Dependencies**: None (Pure C#).
- **Execution**: `dotnet run --project FiloShop.Domain.UnitTests`

### 2. Integration Tests (`FiloShop.IntegrationTests`)
- **Scope**: Application layer (Handlers), Repositories, API Endpoints.
- **Dependencies**: Real Docker containers (via Testcontainers).
- **Prerequisites**: Docker Desktop must be running.
- **Execution**: `dotnet run --project FiloShop.IntegrationTests`

### 3. Architecture Tests (`FiloShop.ArchitectureTests`)
- **Scope**: Enforces dependency rules (e.g., Domain cannot reference Infrastructure).
- **Library**: `NetArchTest.Rules`.
- **Execution**: `dotnet run --project FiloShop.ArchitectureTests`

## 🛠️ How to Writing Integration Tests

We use a **BaseIntegrationTest** class that handles:
- Spinning up PostgreSQL/Redis containers.
- Applying migrations.
- Reseting DB state between tests (via Respawn).

```csharp
public class RegisterUserTests : BaseIntegrationTest
{
    [Test]
    public async Task Should_Register_User_Successfully()
    {
        // Arrange
        var command = new RegisterUserCommand(...);

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify in DB
        var user = await DbContext.Users.FindAsync(result.Value);
        user.Should().NotBeNull();
    }
}
```

## 🚫 Common Pitfalls
1.  **Mocking EVERYTHING**: Don't mock EF Core or MediatR in integration tests. Use the real thing.
2.  **No Docker**: Tests will fail instantly if Docker is not running.
3.  **TUnit Executable**: Remember these are console apps. You can debug them by just setting the project as startup and hitting F5.
