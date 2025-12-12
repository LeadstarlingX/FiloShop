# Contributing to FiloShop

Thank you for your interest in contributing to FiloShop! This guide will help you understand our development process, coding standards, and best practices.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Architecture Guidelines](#architecture-guidelines)
- [Testing Requirements](#testing-requirements)
- [Database Changes](#database-changes)
- [Pull Request Process](#pull-request-process)

## 🤝 Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help others learn and improve
- Follow the Boy Scout Rule: Leave code better than you found it

## 🚀 Getting Started

### Prerequisites

1. **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download)
2. **Docker Desktop** - For running dependencies
3. **Git** - Version control
4. **IDE**: Rider, Visual Studio, or VS Code

### Setup Development Environment

```powershell
# 1. Fork and clone the repository
git clone https://github.com/YOUR-USERNAME/FiloShop.git
cd FiloShop

# 2. Start dependencies
docker compose up -d postgres redis keycloak seq

# 3. Apply migrations
dotnet ef database update -p FiloShop.Infrastructure.Persistence -s FiloShop.Api

# 4. Run the application
dotnet run --project FiloShop.Api

# 5. Access Swagger
# http://localhost:5000/swagger
```

## 🔄 Development Workflow

### Branching Strategy

```
main              ← Production-ready code
  ↓
develop           ← Integration branch
  ↓
feature/xxx       ← Your feature branch
bugfix/xxx        ← Bug fixes
hotfix/xxx        ← Critical production fixes
```

### Creating a Branch

```powershell
# Feature
git checkout -b feature/add-wishlist-aggregate

# Bug fix
git checkout -b bugfix/fix-order-validation

# Hotfix
git checkout -b hotfix/critical-security-fix
```

### Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types**:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `test`: Adding/updating tests
- `chore`: Maintenance tasks

**Examples**:
```
feat(orders): add order cancellation functionality

fix(basket): prevent negative quantity in basket items

docs(readme): update setup instructions for Docker

refactor(domain): extract validation logic to value objects

test(users): add unit tests for user registration
```

## 📐 Coding Standards

### C# Style

Follow [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions):

#### Naming Conventions

```csharp
// ✅ Classes: PascalCase
public class OrderService { }

// ✅ Interfaces: IPascalCase
public interface IOrderRepository { }

// ✅ Methods: PascalCase
public void ProcessOrder() { }

// ✅ Private fields: _camelCase
private readonly IOrderRepository _orderRepository;

// ✅ Parameters: camelCase
public void Create(string userName, int userId) { }

// ✅ Constants: PascalCase
public const int MaxRetryAttempts = 3;

// ✅ Local variables: camelCase
var orderTotal = CalculateTotal();
```

#### File Organization

```csharp
// 1. Using statements (sorted alphabetically)
using FiloShop.Domain.Orders;
using FiloShop.SharedKernel.Results;
using System;

// 2. Namespace
namespace FiloShop.Application.Orders.Commands;

// 3. Class with members in this order:
public class CreateOrderCommandHandler
{
    // Fields
    private readonly IOrderRepository _orderRepository;
    
    // Constructor
    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    // Properties
    public string Name { get; set; }
    
    // Methods
    public async Task<Result<Guid>> Handle(...)
    {
        // Implementation
    }
}
```

### Code Formatting

```csharp
// ✅ Use meaningful names
var activeOrders = await GetActiveOrdersAsync();

// ❌ Avoid abbreviations and single letters
var aos = await GetAOAsync();

// ✅ Use expression-bodied members when concise
public string FullName => $"{FirstName} {LastName}";

// ✅ Use var when type is obvious
var order = new Order();
var orders = await _repository.GetAllAsync();

// ❌ Don't use var when type is not obvious
OrderStatus status = GetOrderStatus();  // Better than: var status = ...
```

## 🏛️ Architecture Guidelines

### Domain Layer

#### Creating Aggregates

```csharp
// ✅ Rich domain model with business logic
public class Order : BaseEntity
{
    private readonly List<OrderItem> _items = new();
    
    private Order() { }  // EF Core
    
    private Order(Guid id, Guid userId, Address shipToAddress)
    {
        Id = id;
        UserId = userId;
        ShipToAddress = shipToAddress;
        Status = OrderStatus.Pending;
    }
    
    // Factory method
    public static Result<Order> Create(User user, Address shipToAddress)
    {
        var order = new Order(Guid.NewGuid(), user.Id, shipToAddress);
        
        // Raise domain event
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));
        
        return order;
    }
    
    // Business logic in methods
    public Result AddItem(CatalogItem item, int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(OrderErrors.InvalidQuantity);
            
        var orderItem = OrderItem.Create(item, quantity).Value;
        _items.Add(orderItem);
        
        return Result.Success();
    }
}
```

#### Creating Value Objects

```csharp
public sealed record Money
{
    public decimal Amount { get; init; }
    public Currency Currency { get; init; }
    
    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public static Result<Money> Create(decimal amount, Currency currency)
    {
        if (amount < 0)
            return Result.Failure<Money>(MoneyErrors.NegativeAmount);
            
        return new Money(amount, currency);
    }
}
```

### Application Layer

#### Commands and Queries

```csharp
// Command
public sealed record CreateOrderCommand(
    Guid UserId,
    Address ShippingAddress,
    List<OrderItemRequest> Items) : IIdempotentRequest<Guid>
{
    public Guid IdempotencyKey { get; init; }
}

// Query
public sealed record GetOrderByIdQuery(Guid OrderId) : ICachedQuery<OrderDto>
{
    public string CacheKey => $"order-{OrderId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}
```

#### Handlers

```csharp
public class CreateOrderCommandHandler 
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<Guid>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Validation
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<Guid>(UserErrors.NotFound);
        
        // 2. Domain logic
        var order = Order.Create(user, request.ShippingAddress);
        if (order.IsFailure)
            return Result.Failure<Guid>(order.Error);
        
        // 3. Persistence
        await _orderRepository.AddAsync(order.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        // 4. Return result
        return order.Value.Id;
    }
}
```

### Infrastructure Layer

### Infrastructure Layer

#### Entity Configurations Policy
> [!IMPORTANT]
> **Strict Rule**: Never use Data Annotations (`[Key]`, `[Required]`, `[Table]`) in the Domain Layer. We follow strict Persistence Ignorance.

All database mappings must be defined in `IEntityTypeConfiguration<T>` classes in the Infrastructure project.

**Do NOT**:
- Add `[Key]` to your Entities.
- Rename columns manually (e.g., `.HasColumnName("user_id")`) - rely on conventions.
- Forget to `IsRequired()` for strings (defaults as nullable without it).

See the [Entity Configurations Deep Dive](./FiloShop.Infrastructure.Persistence/readme_entity_configurations.md) for the mandatory patterns.

```csharp
public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        // Correct usage of ComplexProperty for Value Objects (EF Core 8+)
        builder.ComplexProperty(o => o.ShipToAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street).IsRequired();
            addressBuilder.Property(a => a.City).IsRequired();
            addressBuilder.Property(a => a.Country).IsRequired();
        });
        
        // Correct Relationship mapping
        builder.HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey("OrderId");
    }
}
```

## 🧪 Testing Requirements

### Test Coverage Targets

- **Domain Layer**: 80%+
- **Application Layer**: 70%+
- **Infrastructure Layer**: 50%+

### Unit Tests

```csharp
public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var user = UserTestData.Create();
        var address = AddressTestData.Create();
        
        // Act
        var result = Order.Create(user, address);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(user.Id);
    }
    
    [Fact]
    public void AddItem_WithNegativeQuantity_ShouldFail()
    {
        // Arrange
        var order = OrderTestData.Create();
        var item = CatalogItemTestData.Create();
        
        // Act
        var result = order.AddItem(item, -1);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidQuantity);
    }
}
```

### Integration Tests

```csharp
public class OrdersControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateOrder_WithValidData_Returns201Created()
    {
        // Arrange
        var request = new CreateOrderRequest(...);
        var idempotencyKey = Guid.NewGuid();
        
        // Act
        var response = await Client.PostAsJsonAsync(
            "/api/v1/orders",
            request,
            new Dictionary<string, string>
            {
                ["X-Idempotency-Key"] = idempotencyKey.ToString()
            });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

## 💾 Database Changes

### Creating Migrations

```powershell
# 1. Add entity configuration
# 2. Create migration
dotnet ef migrations add AddWishlistAggregate \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api

# 3. Review generated SQL
# Check the migration file in Migrations/

# 4. Apply migration
dotnet ef database update \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

### Migration Guidelines

- **One logical change** per migration
- **Descriptive names**: `AddWishlistEntity`, not `Update1`
- **Review SQL** before committing
- **Test rollback**: Ensure `Down()` works
- **Never edit** applied migrations

## 📤 Pull Request Process

### Before Submitting

✅ **Checklist**:
- [ ] Code follows style guidelines
- [ ] All tests pass (`dotnet test`)
- [ ] New tests added for new features
- [ ] Documentation updated
- [ ] No compiler warnings
- [ ] Migration tested (if applicable)
- [ ] Idempotency considered for commands

### PR Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
How has this been tested?

## Checklist
- [ ] Tests pass
- [ ] Documentation updated
- [ ] Migration included (if needed)
```

### Review Process

1. **Automated Checks**:
   - Build passes
   - Tests pass
   - Code coverage maintained

2. **Code Review**:
   - At least 1 approval required
   - Address feedback
   - Keep PR focused and small

3. **Merge**:
   - Squash commits
   - Use descriptive merge message

## 🎯 Best Practices

### General

- **SOLID Principles** - Follow them religiously
- **DRY (Don't Repeat Yourself)** - Extract common logic
- **YAGNI (You Aren't Gonna Need It)** - Don't over-engineer
- **KISS (Keep It Simple, Stupid)** - Simplicity wins

### Domain-Driven Design

- **Aggregates** control their boundaries
- **Value Objects** are immutable
- **Domain Events** for cross-aggregate communication
- **Repository** interfaces in Domain layer

### CQRS

- **Commands** return `Result<T>` or `Result`
- **Queries** return DTOs, not entities
- **Handlers** have single responsibility
- **Use MediatR** for all commands/queries

### Error Handling

```csharp
// ✅ Use Result pattern
public Result<Order> Create(...)
{
    if (invalid)
        return Result.Failure<Order>(OrderErrors.InvalidData);
    
    return order;
}

// ❌ Don't throw for business logic
public Order Create(...)
{
    if (invalid)
        throw new InvalidOperationException();  // ❌
}
```

## 📚 Resources

- [Domain Events Guide](./FiloShop.Domain/readme_domain_events.md)
- [Idempotency Guide](./FiloShop.SharedKernel/readme_idempotency.md)
- [Testing Strategy](./readme_testing.md)
- [Migrations Guide](./FiloShop.Infrastructure.Persistence/readme_migrations.md)

## ❓ Questions?

- **GitHub Discussions** - Ask questions
- **Issues** - Report bugs
- **Pull Requests** - Suggest improvements

---

**Thank you for contributing to FiloShop!** 🎉