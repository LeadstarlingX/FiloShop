# Outbox Pattern Implementation Guide

The **Outbox Pattern** ensures reliable event publishing in distributed systems by storing domain events in the same database transaction as the aggregate changes.

## 📖 Overview

### The Problem
When using domain events in a distributed system:
- Events might be lost if the message broker is down
- Dual writes (DB + message broker) can cause inconsistency
- No guarantee that events are published after commit

### The Solution
The Outbox Pattern:
1. Store events in an "outbox" table in the same transaction
2. A background job reads and publishes events
3. Delete/mark processed events

## 🏗️ Architecture

```
Domain Event Raised → OutboxMessage Created → Database Transaction Commits
                                                         ↓
                      Background Job Reads Outbox → Publishes to Broker → Marks Processed
```

## 📁 File Structure

```
FiloShop.SharedKernel/
└── Outbox/
    └── OutboxMessage.cs

FiloShop.Infrastructure.Persistence/
├── AppDbContext/
│   └── ApplicationDbContext.cs  (SaveChanges override)
└── EntitiesConfigurations/
    └── SystemEntities/
        └── OutboxMessageConfiguration.cs

FiloShop.Infrastructure.Services/
└── Providers/
    └── Outbox/
        ├── OutboxOptions.cs
        ├── ProcessOutboxMessagesJob.cs
        └── PublishDomainEventsService.cs
```

## 🔧 Implementation

### 1. OutboxMessage Entity

Located in `FiloShop.SharedKernel/Outbox/OutboxMessage.cs`:

```csharp
public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    public Guid Id { get; private set; }
    public string Type { get; private set; }          // Event type name
    public string Content { get; private set; }       // Serialized event (JSON)
    public DateTime OccurredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }
}
```

### 2. Automatic Conversion in DbContext

In `ApplicationDbContext.SaveChangesAsync`:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    AddDomainEventsAsOutboxMessages();  // ← Convert events before commit
    return await base.SaveChangesAsync(cancellationToken);
}

private void AddDomainEventsAsOutboxMessages()
{
    var outboxMessages = ChangeTracker
        .Entries<BaseEntity>()
        .Select(entry => entry.Entity)
        .SelectMany(entity =>
        {
            var domainEvents = entity.GetDomainEvents();
            entity.ClearDomainEvents();
            return domainEvents;
        })
        .Select(domainEvent => new OutboxMessage(
            Guid.NewGuid(),
            _dateTimeProvider.UtcNow,
            domainEvent.GetType().Name,
            JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
        .ToList();
        
    AddRange(outboxMessages);
}
```

### 3. Background Processing with Quartz

Configure in `appsettings.json`:

```json
{
  "Outbox": {
    "IntervalInSeconds": 10,
    "BatchSize": 20
  }
}
```

Job implementation:

```csharp
[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly OutboxOptions _options;

    public async Task Execute(IJobExecutionContext context)
    {
        // 1. Fetch unprocessed messages
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(_options.BatchSize)
            .ToListAsync();

        // 2. Deserialize and publish each
        foreach (var message in messages)
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                message.Content,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            await _publisher.Publish(domainEvent);

            message.ProcessedOnUtc = DateTime.UtcNow;
        }

        // 3. Save processed status
        await _dbContext.SaveChangesAsync();
    }
}
```

## ⚙️ Configuration

### Register Quartz Job

In `DependencyInjection.cs`:

```csharp
services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

services.AddQuartz(configure =>
{
    var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

    configure
        .AddJob<ProcessOutboxMessagesJob>(jobKey)
        .AddTrigger(trigger =>
            trigger.ForJob(jobKey)
                .WithSimpleSchedule(schedule =>
                    schedule.WithIntervalInSeconds(10)
                        .RepeatForever()));
});

services.AddQuartzHostedService(options => 
    options.WaitForJobsToComplete = true);
```

## 🎯 Usage Example

### 1. Raise Domain Event in Aggregate

```csharp
public static Result<Order> Create(User user, Address shipToAddress)
{
    var order = new Order(Guid.NewGuid(), user.Id, shipToAddress);
    
    // Raise event - it will be converted to OutboxMessage automatically
    order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));
    
    return order;
}
```

### 2. Event Gets Stored in Outbox

When `SaveChangesAsync()` is called:
```sql
INSERT INTO "OutboxMessage" 
("Id", "Type", "Content", "OccurredOnUtc", "ProcessedOnUtc", "Error")
VALUES 
('...', 'OrderCreatedDomainEvent', '{"OrderId":"..."}', '2024-01-01', NULL, NULL);
```

### 3. Background Job Publishes Event

Every 10 seconds, Quartz job:
- Reads unprocessed messages
- Publishes via MediatR
- Marks as processed

### 4. Event Handlers Execute

```csharp
public class OrderCreatedDomainEventHandler 
    : INotificationHandler<OrderCreatedDomainEvent>
{
    public async Task Handle(
        OrderCreatedDomainEvent notification, 
        CancellationToken cancellationToken)
    {
        // Send confirmation email
        // Update analytics
        // Notify warehouse
    }
}
```

## ✅ Benefits

1. **Guaranteed Delivery** - Events stored in DB transaction
2. **Retry Logic** - Failed events can be retried
3. **Exactly-Once** - Deduplication possible via message ID
4. **Audit Trail** - All events are logged
5. **Decoupling** - Publishers don't know about subscribers

## 🔍 Database Schema

```sql
CREATE TABLE "OutboxMessage" (
    "Id" UUID PRIMARY KEY,
    "Type" TEXT NOT NULL,
    "Content" JSONB NOT NULL,
    "OccurredOnUtc" TIMESTAMP NOT NULL,
    "ProcessedOnUtc" TIMESTAMP NULL,
    "Error" TEXT NULL
);

CREATE INDEX "ix_outbox_messages_processed" 
ON "OutboxMessage" ("ProcessedOnUtc") 
WHERE "ProcessedOnUtc" IS NULL;
```

## ⚠️ Important Notes

- **Performance**: Add index on `ProcessedOnUtc` for faster queries
- **Cleanup**: Periodically delete old processed messages
- **Error Handling**: Store errors in `Error` column for debugging
- **Idempotency**: Event handlers should be idempotent
- **Serialization**: Use `TypeNameHandling.All` for polymorphism

## 🧹 Cleanup Strategy

Add a cleanup job to delete old processed messages:

```csharp
var cutoffDate = DateTime.UtcNow.AddDays(-30);

await _dbContext.OutboxMessages
    .Where(m => m.ProcessedOnUtc < cutoffDate)
    .ExecuteDeleteAsync();
```

## 🚀 Best Practices

1. **Keep Events Small** - Only essential data
2. **Version Events** - Support schema evolution
3. **Monitor Processing** - Track failed messages
4. **Set Batch Size** - Balance throughput vs. latency
5. **Use Transactions** - Ensure atomicity

---

The Outbox Pattern provides **reliable event publishing** without distributed transactions. It's essential for maintaining consistency in event-driven architectures.
