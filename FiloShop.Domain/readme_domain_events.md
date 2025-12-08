# Domain Events Guide

**Domain Events** represent something significant that happened in the domain. They enable loose coupling between aggregates and support event-driven architectures.

## 📖 Overview

### What are Domain Events?

Domain events are notifications about state changes in your domain. They:
- Represent facts that have occurred
- Are immutable (past tense naming)
- Contain only essential data
- Enable decoupled communication

### Why Use Domain Events?

✅ **Decouple aggregates** - No direct dependencies  
✅ **Single Responsibility** - Aggregates focus on their logic  
✅ **Audit trail** - Track what happened when  
✅ **Integration** - Notify external systems  
✅ **Eventual consistency** - Update read models asynchronously  

## 🏗️ Architecture

```
Aggregate raises event → Stored in entity → SaveChanges → Outbox → Published → Handlers execute
```

## 📁 File Structure

### Domain Events
```
FiloShop.Domain/
├── Users/Events/
│   └── UserCreatedDomainEvent.cs
├── Orders/Events/
│   └── OrderCreatedDomainEvent.cs
├── CatalogItems/Events/
│   └── CatalogItemCreatedDomainEvent.cs
└── Baskets/Events/
    └── BasketCreatedDomainEvent.cs
```

### Event Handlers
```
FiloShop.Application/
├── Users/Events/
│   └── UserCreatedDomainEventHandler.cs
├── Orders/Events/
│   └── OrderCreatedDomainEventHandler.cs
```

## 🔧 Implementation

### 1. Define a Domain Event

In `FiloShop.Domain/Orders/Events/`:

```csharp
using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.Orders.Events;

public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount) : IDomainEvent;
```

**Key Points**:
- **Record** type (immutable)
- **Sealed** (no inheritance)
- **Past tense** naming
- Implements `IDomainEvent` marker interface
- Contains only essential data

### 2. Raise Event in Aggregate

In `Order.cs`:

```csharp
public static Result<Order> Create(User user, Address shipToAddress)
{
    var order = new Order(Guid.NewGuid(), user.Id, shipToAddress);
    
    // Raise the domain event
    order.RaiseDomainEvent(new OrderCreatedDomainEvent(
        order.Id,
        user.Id,
        order.TotalAmount));
    
    return order;
}
```

### 3. Base Entity Handles Storage

`BaseEntity` provides event management:

```csharp
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

### 4. Create Event Handler

In `FiloShop.Application/Orders/Events/`:

```csharp
using MediatR;
using FiloShop.Domain.Orders.Events;

public class OrderCreatedDomainEventHandler 
    : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<OrderCreatedDomainEventHandler> _logger;

    public OrderCreatedDomainEventHandler(
        IEmailService emailService,
        ILogger<OrderCreatedDomainEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(
        OrderCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Order {OrderId} was created for user {UserId}",
            notification.OrderId,
            notification.UserId);

        // Send confirmation email
        await _emailService.SendOrderConfirmationAsync(
            notification.OrderId,
            cancellationToken);

        // Other side effects...
    }
}
```

## 🎯 Publishing Flow

### 1. Command Execution

```csharp
// In CreateOrderCommandHandler
var order = Order.Create(user, request.ShippingAddress);

// Event is stored in order._domainEvents
await _orderRepository.AddAsync(order);

// SaveChanges triggers publishing
await _unitOfWork.SaveChangesAsync();
```

### 2. SaveChanges Converts to Outbox

In `ApplicationDbContext`:

```csharp
public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
{
    // Get all domain events from tracked entities
    var domainEvents = ChangeTracker
        .Entries<BaseEntity>()
        .SelectMany(e => e.Entity.GetDomainEvents())
        .ToList();

    // Clear events from entities
    ChangeTracker.Entries<BaseEntity>()
        .ToList()
        .ForEach(e => e.Entity.ClearDomainEvents());

    // Convert to outbox messages
    var outboxMessages = domainEvents.Select(e => new OutboxMessage(
        Guid.NewGuid(),
        DateTime.UtcNow,
        e.GetType().Name,
        JsonConvert.SerializeObject(e)));

    AddRange(outboxMessages);

    // Save aggregate + outbox in same transaction
    return await base.SaveChangesAsync(cancellationToken);
}
```

### 3. Background Job Publishes

The Outbox processor:
1. Reads unprocessed outbox messages
2. Deserializes events
3. Publishes via MediatR
4. Marks as processed

### 4. Handlers Execute

All registered `INotificationHandler<TEvent>` execute asynchronously.

## 🎨 Event Naming Conventions

| ✅ Good | ❌ Bad |
|---------|--------|
| `OrderCreatedDomainEvent` | `CreateOrderEvent` |
| `UserRegisteredDomainEvent` | `RegisterUserEvent` |
| `PaymentProcessedDomainEvent` | `ProcessPayment` |
| `CatalogItemDeletedDomainEvent` | `CatalogItemDeletion` |

**Rules**:
- **Past tense** - Something that happened
- **DomainEvent** suffix - Clear intent
- **Descriptive** - What changed

## 📝 Real-World Examples

### Example 1: Order Created

**Event**:
```csharp
public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    Guid UserId,
    Guid[] CatalogItemIds) : IDomainEvent;
```

**Handlers**:
```csharp
// Send email
public class SendOrderConfirmationEmailHandler { }

// Update analytics
public class UpdateOrderAnalyticsHandler { }

// Notify warehouse
public class NotifyWarehouseHandler { }

// Create invoice
public class CreateInvoiceHandler { }
```

### Example 2: User Registered

**Event**:
```csharp
public sealed record UserRegisteredDomainEvent(
    Guid UserId,
    string Email) : IDomainEvent;
```

**Handlers**:
```csharp
// Send welcome email
public class SendWelcomeEmailHandler { }

// Create loyalty account
public class CreateLoyaltyAccountHandler { }

// Log to analytics
public class TrackUserRegistrationHandler { }
```

## ✅ Best Practices

### 1. Keep Events Small
```csharp
// ✅ Good - minimal data
public record OrderCreatedDomainEvent(Guid OrderId, Guid UserId);

// ❌ Bad - too much data
public record OrderCreatedDomainEvent(Order Order, User User, List<OrderItem> Items);
```

### 2. Events are Facts
```csharp
// ✅ Good - fact
public record OrderShippedDomainEvent(Guid OrderId, string TrackingNumber);

// ❌ Bad - command
public record ShipOrderDomainEvent(Guid OrderId);
```

### 3. Handler Idempotency
```csharp
public async Task Handle(OrderCreatedDomainEvent notification, ...)
{
    // ✅ Check if already processed
    var existingInvoice = await _invoiceRepository
        .GetByOrderIdAsync(notification.OrderId);
    
    if (existingInvoice is not null) return;

    // Process event...
}
```

### 4. Error Handling
```csharp
public async Task Handle(OrderCreatedDomainEvent notification, ...)
{
    try
    {
        await _emailService.SendAsync(...);
    }
    catch (Exception ex)
    {
        // ✅ Log and continue - don't fail entire process
        _logger.LogError(ex, "Failed to send email for order {OrderId}", 
            notification.OrderId);
    }
}
```

### 5. Event Versioning
```csharp
// V1
public record OrderCreatedDomainEvent(Guid OrderId, Guid UserId);

// V2 - add property
public record OrderCreatedDomainEventV2(
    Guid OrderId, 
    Guid UserId,
    decimal TotalAmount);  // New field
```

## ⚠️ Common Pitfalls

### ❌ Raising Events Outside Aggregate
```csharp
// Bad - event raised in application layer
var order = await _repository.GetByIdAsync(orderId);
mediator.Publish(new Order UpdatedDomainEvent(orderId));  // ❌
```

### ✅ Raise in Aggregate
```csharp
// Good - aggregate owns its events
public void UpdateStatus(OrderStatus newStatus)
{
    Status = newStatus;
    RaiseDomainEvent(new OrderStatusChangedDomainEvent(Id, newStatus));
}
```

### ❌ Direct Aggregate Communication
```csharp
// Bad - direct dependency
var user = await _userRepository.GetByIdAsync(order.UserId);
user.UpdateLoyaltyPoints(order.TotalAmount);  // ❌
```

### ✅ Use Events
```csharp
// Good - decoupled via events
order.Complete();  // Raises OrderCompletedDomainEvent

// Handler updates loyalty points
public class UpdateLoyaltyPointsHandler : INotificationHandler<OrderCompletedDomainEvent> { }
```

## 🔍 Testing Events

```csharp
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
    domainEvent.UserId.Should().Be(user.Id);
}
```

## 📊 Monitoring Events

Track event processing:
- **Published count** - How many events
- **Processing time** - Handler performance
- **Failure rate** - Errors in handlers
- **Retry count** - Failed -> Retried

---

Domain Events enable **loosely coupled, event-driven architectures** while maintaining strong consistency where it matters.
