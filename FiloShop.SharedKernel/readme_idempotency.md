# 📘 ApiResponse & Idempotency System - Documentation

## 🎯 Overview

This implementation provides two critical production features:
1. **Consistent API Responses** - Standardized response format across all endpoints
2. **Idempotency Protection** - Prevents duplicate command execution for the same request

---

## 📁 Files Added

### **SharedKernel Layer**

#### 1. `API/ApiResponse.cs`
**Purpose:** Standardized API response wrapper

**What it does:**
- Wraps all API responses in a consistent format
- Provides `success`, `data`, `error`, and `meta` fields
- Supports both generic `ApiResponse<T>` and non-generic `ApiResponse`
- Includes pagination metadata support

**Example Response:**
```json
{
  "success": true,
  "data": "9f8c7b6a-5e4d-3c2b-1a0f-9e8d7c6b5a4f",
  "meta": {
    "timestamp": "2024-12-06T10:30:00Z",
    "traceId": "00-abc123-def456-01"
  }
}
```

**Usage:**
```csharp
// Success
return Ok(ApiResponse<Guid>.Ok(medicationId));

// Error
return BadRequest(ApiResponse<Guid>.Fail(error));
```

---

#### 2. `CQRS/Commands/IIdempotentRequest.cs`
**Purpose:** Marker interface for commands that require idempotency

**What it does:**
- Marks a command as idempotent
- Forces the command to include an `IdempotencyKey` property
- Works with MediatR pipeline behavior

**Usage:**
```csharp
public sealed record CreateMedicationCommand(
    Guid IdempotencyKey,
    string Name
) : IIdempotentRequest<Guid>;
```

---

#### 3. `Idempotency/IdempotencyRecord.cs`
**Purpose:** Domain entity representing a cached idempotent request

**What it does:**
- Stores the idempotency key, request name, and serialized response
- Tracks when the request was first processed
- Used by the store to retrieve cached responses

**Properties:**
- `IdempotencyKey` - Unique identifier for the request (from client)
- `RequestName` - Name of the command (e.g., "CreateMedicationCommand")
- `SerializedResponse` - JSON of the original response
- `CreatedAt` - Timestamp of first execution

---

#### 4. `Idempotency/IIdempotencyStore.cs`
**Purpose:** Interface for idempotency persistence

**What it does:**
- Defines contract for storing and retrieving idempotency records
- Abstracts database implementation details

**Methods:**
- `GetByKeyAsync(Guid key)` - Retrieves cached response if exists
- `SaveAsync(IdempotencyRecord)` - Saves new idempotency record

---

#### 5. `Behaviors/IdempotentCommandBehavior.cs`
**Purpose:** MediatR pipeline behavior that intercepts idempotent commands

**What it does:**
- Automatically checks if a request was already processed
- Returns cached response if idempotency key exists
- Executes command and caches response if new request
- Logs all idempotency operations

**Flow:**
```
1. Command arrives with IdempotencyKey
2. Check store for existing record
3. If found → Return cached response
4. If not found → Execute command
5. Cache response with IdempotencyKey
6. Return response
```

---

### **Infrastructure.Persistence Layer**

#### 6. `Idempotency/IdempotencyStore.cs`
**Purpose:** PostgreSQL implementation of `IIdempotencyStore`

**What it does:**
- Uses EF Core to read/write idempotency records
- Performs database queries to check for cached responses
- Handles database transactions for saving records

**Methods:**
```csharp
GetByKeyAsync(Guid key)  // SELECT WHERE idempotency_key = @key
SaveAsync(record)        // INSERT INTO idempotency_records
```

---

#### 7. `EntitiesConfigurations/IdempotencyRecordConfiguration.cs`
**Purpose:** EF Core entity configuration for `IdempotencyRecord`

**What it does:**
- Maps `IdempotencyRecord` to `idempotency_records` table
- Configures column names, types, and constraints
- Sets up database indexes for performance
- Uses `jsonb` type for PostgreSQL to store serialized response

**Table Schema:**
```sql
CREATE TABLE idempotency_records (
    idempotency_key UUID PRIMARY KEY,
    request_name VARCHAR(500) NOT NULL,
    serialized_response JSONB NOT NULL,
    created_at TIMESTAMP NOT NULL
);

CREATE INDEX ix_idempotency_records_created_at 
    ON idempotency_records(created_at);
```

---

## 🔧 How It Works Together

### Request Flow (Happy Path - New Request)
```
1. Client sends POST with X-Idempotency-Key: {guid}
   ↓
2. Controller creates command with IdempotencyKey
   ↓
3. MediatR sends command through pipeline
   ↓
4. IdempotentCommandBehavior intercepts
   ↓
5. Behavior checks IdempotencyStore.GetByKeyAsync()
   ↓
6. Key not found → proceed to handler
   ↓
7. Handler executes business logic
   ↓
8. Behavior caches response via IdempotencyStore.SaveAsync()
   ↓
9. Response returned to client
```

### Request Flow (Duplicate Request)
```
1. Client sends SAME POST with SAME X-Idempotency-Key
   ↓
2. Controller creates command with same IdempotencyKey
   ↓
3. MediatR sends command through pipeline
   ↓
4. IdempotentCommandBehavior intercepts
   ↓
5. Behavior checks IdempotencyStore.GetByKeyAsync()
   ↓
6. Key FOUND → deserialize cached response
   ↓
7. ❌ Handler NOT executed (business logic skipped)
   ↓
8. Cached response returned to client
```

---

## 🚀 Setup Steps

### Step 1: Add to `ApplicationDbContext.cs`
```csharp
public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
```

### Step 2: Register Services in `Program.cs`
```csharp
// MediatR with Idempotency Behavior
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Application.DependencyInjection).Assembly);
    cfg.AddOpenBehavior(typeof(IdempotentCommandBehavior<,>));
});

// Idempotency Store
builder.Services.AddScoped<IIdempotencyStore, IdempotencyStore>();
```

### Step 3: Create Database Migration
```bash
dotnet ef migrations add AddIdempotencyTable -p LeroTreat.Infrastructure.Persistence
dotnet ef database update -p LeroTreat.Infrastructure.Persistence
```

---

## 💡 Usage Examples

### Example 1: Idempotent Command
```csharp
// Application Layer
public sealed record CreateMedicationCommand(
    Guid IdempotencyKey,
    string Name,
    string Barcode
) : IIdempotentRequest<Guid>;

public sealed class CreateMedicationCommandHandler : ICommandHandler<CreateMedicationCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMedicationCommand request, CancellationToken ct)
    {
        var medication = Medication.Create(/*...*/);
        await _repository.AddAsync(medication.Value, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success(medication.Value.Id);
    }
}
```

### Example 2: Controller Endpoint
```csharp
// Presentation Layer
[HttpPost]
public async Task<IActionResult> CreateMedication(
    [FromBody] CreateMedicationRequest request,
    [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey)
{
    var command = new CreateMedicationCommand(
        idempotencyKey,
        request.Name,
        request.Barcode);

    var result = await _sender.Send(command);

    return result.IsSuccess 
        ? Ok(ApiResponse<Guid>.Ok(result.Value))
        : BadRequest(ApiResponse<Guid>.Fail(result.Error));
}
```

### Example 3: Client Request
```bash
# First request
curl -X POST https://localhost:7271/api/medications \
  -H "X-Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '{"name":"Aspirin","barcode":"123"}'

# Response (201 Created)
{
  "success": true,
  "data": "9f8c7b6a-5e4d-3c2b-1a0f-9e8d7c6b5a4f",
  "meta": {
    "timestamp": "2024-12-06T10:30:00Z"
  }
}

# Duplicate request (same idempotency key)
curl -X POST https://localhost:7271/api/medications \
  -H "X-Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '{"name":"Aspirin","barcode":"123"}'

# Response (200 OK - same response, no duplicate created)
{
  "success": true,
  "data": "9f8c7b6a-5e4d-3c2b-1a0f-9e8d7c6b5a4f",
  "meta": {
    "timestamp": "2024-12-06T10:30:00Z"
  }
}
```

---

## ⚠️ Important Notes

### When to Use Idempotency
Use for commands that:
- ✅ Create resources (POST)
- ✅ Update resources (PUT/PATCH)
- ✅ Process payments
- ✅ Send emails/notifications
- ✅ Any operation that should NOT be repeated

Do NOT use for:
- ❌ Queries (GET requests)
- ❌ Truly idempotent operations (DELETE by ID)

### Client Responsibilities
The client MUST:
1. Generate a unique `Guid` for each request
2. Include it in `X-Idempotency-Key` header
3. Retry with the SAME key if request fails
4. Generate a NEW key for different operations

Example (JavaScript):
```javascript
const idempotencyKey = crypto.randomUUID();

fetch('/api/medications', {
  method: 'POST',
  headers: {
    'X-Idempotency-Key': idempotencyKey,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify(data)
});
```

### Retention Policy
Idempotency records are stored forever by default. Consider:
- Adding a cleanup job (delete records older than 24-48 hours)
- Adding `CompletedAt` timestamp for tracking
- Using Quartz.NET for scheduled cleanup

---

## 🎯 Benefits

### ApiResponse<T>
✅ Consistent response format across all endpoints  
✅ Easy error handling on client side  
✅ Supports pagination metadata  
✅ Includes tracing for debugging  
✅ Type-safe with generics  

### Idempotency System
✅ Prevents duplicate charges/operations  
✅ Safe to retry failed requests  
✅ Database-level consistency  
✅ Automatic caching via MediatR behavior  
✅ Works with distributed systems  
✅ No changes to existing handlers  

---

## 🔍 Troubleshooting

### Issue: Idempotency not working
**Check:**
1. Is `IdempotentCommandBehavior` registered in MediatR?
2. Does your command implement `IIdempotentRequest<T>`?
3. Is the database table created?
4. Is `IIdempotencyStore` registered in DI?

### Issue: Duplicate records still created
**Check:**
1. Is the same `IdempotencyKey` being sent?
2. Check logs for "already processed" message
3. Verify database constraint on `idempotency_key`

### Issue: Cached response is wrong
**Solution:**
- Idempotency cache is permanent for that key
- Client must generate NEW key for NEW operation
- Never reuse keys across different operations

---

## 📚 References

- [Idempotency Pattern - Microsoft](https://learn.microsoft.com/en-us/azure/architecture/patterns/idempotent-consumer)
- [Stripe's Idempotency Guide](https://stripe.com/docs/api/idempotent_requests)
- [RFC 7807 - Problem Details](https://datatracker.ietf.org/doc/html/rfc7807)
