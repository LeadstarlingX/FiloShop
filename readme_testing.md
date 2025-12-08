# Testing Strategy Guide

This guide covers the **testing approach** for FiloShop, including unit tests, integration tests, architecture tests, and testing best practices.

## 📖 Testing Pyramid

```
         /

\
        /  E2E  \          ← Few, slow, expensive
       /---------\
      /Integration\         ← Some, moderate speed
     /-------------\
    /  Unit Tests   \       ← Many, fast, cheap
   /-----------------\
```

## 🧪 Test Projects

### 1. Unit Tests - `FiloShop.Domain.UnitTests`

**Purpose**: Test domain logic in isolation

**What to Test**:
- Aggregate behavior
- Value object validation
- Domain event raising
- Business rule enforcement

**Example**:
```csharp
public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var user = UserTestData.Create();
        var address = AddressTestData.Create();

        // Act
        var result = Order.Create(user, address);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(user.Id);
        result.Value.ShipToAddress.Should().Be(address);
    }

    [Fact]
    public void Create_ShouldRaiseOrderCreatedEvent()
    {
        // Arrange
        var user = UserTestData.Create();
        var address = AddressTestData.Create();

        // Act
        var order = Order.Create(user, address).Value;

        // Assert
        var domainEvent = order.GetDomainEvents()
            .OfType<OrderCreatedDomainEvent>()
            .SingleOrDefault();

        domainEvent.Should().NotBeNull();
        domainEvent!.OrderId.Should().Be(order.Id);
    }
}
```

### 2. Integration Tests - `FiloShop.IntegrationTests`

**Purpose**: Test full request/response flow

**What to Test**:
- API endpoints
- Database interactions
- External service integrations
- Full CQRS pipeline

**Setup**:
```csharp
public class IntegrationTestBase : IAsyncLifetime
{
    protected HttpClient Client { get; private set; }
    protected WebApplicationFactory<Program> Factory { get; private set; }

    public async Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace real DB with in-memory
                    services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

        Client = Factory.CreateClient();
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        return Task.CompletedTask;
    }
}
```

**Example**:
```csharp
public class CatalogItemsControllerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetCatalogItem_WithValidId_ReturnsOk()
    {
        // Arrange
        var catalogItem = await CreateCatalogItemAsync();

        // Act
        var response = await Client.GetAsync($"/api/v1/catalogitems/{catalogItem.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse<CatalogItemDto>>(content);
        
        apiResponse.Should().NotBeNull();
        apiResponse!.IsSuccess.Should().BeTrue();
        apiResponse.Data!.Id.Should().Be(catalogItem.Id);
    }

    [Fact]
    public async Task CreateCatalogItem_WithIdempotencyKey_Preventsduplicates()
    {
        // Arrange
        var request = new CreateCatalogItemRequest(...);
        var idempotencyKey = Guid.NewGuid();

        // Act - Send same request twice with same key
        var response1 = await PostWithIdempotencyAsync(request, idempotencyKey);
        var response2 = await PostWithIdempotencyAsync(request, idempotencyKey);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.Created);
        response2.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // Both should return the same ID (idempotent)
        var id1 = await GetIdFromResponse(response1);
        var id2 = await GetIdFromResponse(response2);
        id1.Should().Be(id2);
    }
}
```

### 3. Architecture Tests - `FiloShop.ArchitectureTests`

**Purpose**: Enforce architectural boundaries

**What to Test**:
- Layer dependencies
- Naming conventions
- Folder structure
- Interface segregation

**Example**:
```csharp
public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        // Arrange
        var domainAssembly = typeof(Order).Assembly;
        var forbiddenAssemblies = new[]
        {
            "FiloShop.Infrastructure.Persistence",
            "FiloShop.Infrastructure.Services"
        };

        // Act
        var result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOnAll(forbiddenAssemblies)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_ShouldHaveCorrectNaming()
    {
        // Arrange
        var assembly = typeof(CreateOrderCommandHandler).Assembly;

        // Act
        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
```

### 4. Naming Tests - `FiloShop.NamingTests`

**Purpose**: Enforce naming conventions

**Example**:
```csharp
public class NamingConventionTests
{
    [Fact]
    public void Commands_ShouldEndWithCommand()
    {
        var assembly = typeof(CreateOrderCommand).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_ShouldEndWithDomainEvent()
    {
        var assembly = typeof(OrderCreatedDomainEvent).Assembly;

        var result = Types.InAssembly(assembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
```

## 🛠️ Tools & Libraries

### Testing Frameworks
- **xUnit** - Test framework
- **FluentAssertions** - Readable assertions
- **NetArchTest** - Architecture rules
- **Moq** - Mocking framework
- **Bogus** - Fake data generation

### Test Isolation
- **In-Memory Database** - For integration tests
- **TestContainers** - Real PostgreSQL in Docker (optional)
- **WebApplicationFactory** - API testing

## 🎯 What to Test

### Domain Layer
✅ **Test**:
- Aggregate creation logic
- Business rule validation
- Domain events raised
- Value object constraints

❌ **Don't Test**:
- Property getters/setters
- Auto-properties
- Constructor parameter assignment

### Application Layer
✅ **Test**:
- Command/query handlers
- Validation rules
- Error handling
- State transitions

❌ **Don't Test**:
- MediatR infrastructure
- DI container configuration

### Infrastructure Layer
✅ **Test**:
- Repository implementations
- External API clients
- Data mapping logic

❌ **Don't Test**:
- EF Core internals
- Third-party libraries

## ✅ Best Practices

### 1. Arrange-Act-Assert Pattern
```csharp
[Fact]
public void Example Test()
{
    // Arrange - Setup
    var user = CreateUser();

    // Act - Execute
    var result = user.Activate();

    // Assert - Verify
    result.IsSuccess.Should().BeTrue();
}
```

### 2. Use Test Data Builders
```csharp
public static class UserTestData
{
    public static User Create(
        string firstName = "John",
        string lastName = "Doe",
        string email = "john@example.com")
    {
        return User.Create(
            FirstName.Create(firstName).Value,
            LastName.Create(lastName).Value,
            Email.Create(email).Value).Value;
    }
}
```

### 3. One Assert Per Test (mostly)
```csharp
// ✅ Good - focused
[Fact]
public void Create_WithValidData_ShouldSucceed()
{
    var result = Order.Create(user, address);
    result.IsSuccess.Should().BeTrue();
}

// ❌ Bad - too many assertions
[Fact]
public void Create_TestsEverything()
{
    var result = Order.Create(user, address);
    result.IsSuccess.Should().BeTrue();
    result.Value.UserId.Should().Be(user.Id);
    result.Value.Items.Should().BeEmpty();
    result.Value.Status.Should().Be(OrderStatus.Pending);
    // ... 10 more assertions
}
```

### 4. Test Naming Conventions
```csharp
// Pattern: MethodName_Scenario_ExpectedBehavior

[Fact]
public void Create_WithNullAddress_ShouldReturnFailure()

[Fact]
public void UpdateStatus_ToCancelled_ShouldRaiseOrderCancelledEvent()

[Fact]
public void AddItem_WhenDuplicate_ShouldIncreaseQuantity()
```

### 5. Use Theory for Multiple Scenarios
```csharp
[Theory]
[InlineData("", false)]
[InlineData("a", false)]
[InlineData("ab", true)]
[InlineData("valid", true)]
[InlineData("a".PadRight(101, 'x'), false)]
public void Create_WithVariousLengths_ValidatesCorrectly(string value, bool expectedSuccess)
{
    var result = Name.Create(value);
    result.IsSuccess.Should().Be(expectedSuccess);
}
```

## 🚀 Running Tests

### All Tests
```powershell
dotnet test
```

### Specific Project
```powershell
dotnet test FiloShop.Domain.UnitTests
```

### With Coverage
```powershell
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Filter by Name
```powershell
dotnet test --filter "FullyQualifiedName~Order"
```

## 📊 Code Coverage

### Target Coverage
- **Domain**: 80%+
- **Application**: 70%+
- **Infrastructure**: 50%+

### Generate Report
```powershell
# Install tool
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Generate HTML report
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report
```

## ⚠️ Common Pitfalls

### ❌ Testing Implementation Details
```csharp
// Bad - testing private method indirectly
[Fact]
public void PrivateMethod_ShouldWork()
{
    // Don't test private methods!
}
```

### ✅ Test Public Behavior
```csharp
// Good - testing observable behavior
[Fact]
public void Create_WithValidData_ShouldCreateOrder()
{
    var order = Order.Create(user, address).Value;
    order.Should().NotBeNull();
}
```

### ❌ Excessive Mocking
```csharp
// Bad - too many mocks
var mock1 = new Mock<IService1>();
var mock2 = new Mock<IService2>();
var mock3 = new Mock<IService3>();
// ...
```

### ✅ Real Objects When Possible
```csharp
// Good - use real value objects
var money = Money.Create(100, Currency.USD).Value;
var address = Address.Create(...).Value;
```

---

A solid testing strategy ensures **confidence in changes** and enables **safe refactoring**. Focus on business logic and behaviors, not implementation details.
