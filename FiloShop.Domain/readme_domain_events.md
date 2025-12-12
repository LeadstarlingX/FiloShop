# Domain Events Guide

This guide describes how to implement and use **Domain Events** in FiloShop.

> [!IMPORTANT]
> Domain Events are the ONLY allowed mechanism for side effects across aggregate boundaries. Never modify two aggregates in the same transaction.

## 📝 How to Create a Domain Event

1.  **Define the Record**: Create a `sealed record` in the `Events` folder of your aggregate.
2.  **Naming Convention**: `[Entity][Action]DomainEvent` (e.g., `OrderCreatedDomainEvent`).
3.  **Implement Interface**: Must implement `IDomainEvent`.
4.  **Content**: Include minimal data needed for the handler (usually IDs).

### Example

```csharp
namespace FiloShop.Domain.Orders.Events;

public sealed record OrderCreatedDomainEvent(Guid OrderId) : IDomainEvent;
```

## 📣 How to Raise an Event

Events are raised **inside the Domain Entity** method where the state change happens.

```csharp
public class Order : BaseEntity, IAggregateRoot
{
    public static Result<Order> Create(User user, Address address)
    {
        var order = new Order(Guid.NewGuid(), user.Id, address);

        // ✅ Raise event immediately after state change
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));

        return order;
    }
}
```

## 👂 How to Handle an Event

Handlers live in the **Application Layer** and usually publish to the Outbox.

```csharp
internal sealed class OrderCreatedDomainEventHandler
    : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly IOutboxService _outbox;

    public OrderCreatedDomainEventHandler(IOutboxService outbox)
    {
        _outbox = outbox;
    }

    public async Task Handle(
        OrderCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        // ✅ Convert to integration event for external systems
        var integrationEvent = new OrderCreatedIntegrationEvent(notification.OrderId);

        await _outbox.PublishAsync(integrationEvent, cancellationToken);
    }
}
```

## 📏 Strict Rules for Contributors

1.  **Immutability**: Events must be `sealed record` types.
2.  **Past Tense**: Explicitly name events in the past tense (`Created`, `Updated`).
3.  **Minimal Payload**: Don't put entire objects in events; use IDs.
4.  **No Logic in Constructors**: Event records should be simple data containers.
