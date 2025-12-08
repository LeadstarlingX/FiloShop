# FiloShop.API

The **API** layer is the entry point for HTTP requests. It handles routing, request/response transformation, and delegates business logic to the Application layer via MediatR.

## 🎯 Responsibilities

- **HTTP Endpoints** - RESTful API controllers
- **Request Validation** - Basic model binding
- **API Versioning** - Multiple API versions support
- **Middleware** - Exception handling, logging
- **Swagger/OpenAPI** - API documentation
- **Startup Configuration** - DI container setup

## 📁 Structure

```
FiloShop.API/
├── Controllers/
│   ├── CatalogItems/
│   ├── Users/
│   ├── Orders/
│   └── Baskets/
├── Extensions/
│   └── ApplicationBuilderExtensions.cs
├── Middleware/
├── Program.cs
├── Startup.cs
└── appsettings.json
```

## 🚀 Key Features

### API Versioning
- Supports multiple API versions
- URL-based versioning (`/api/v1/...`)
- Swagger UI for all versions

### Standardized Responses
All endpoints return `ApiResponse<T>`:
```csharp
{
  "data": { ... },
  "isSuccess": true,
  "error": null
}
```

### Idempotency Support
Commands accept `X-Idempotency-Key` header:
```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder(
    [FromBody] CreateOrderRequest request,
    [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey)
{
    var command = new CreateOrderCommand(...) 
    { 
        IdempotencyKey = idempotencyKey 
    };
    var result = await _sender.Send(command);
    return result.Match(Ok, BadRequest);
}
```

### Exception Handling
Global exception middleware converts:
- `ValidationException` → 400 Bad Request
- `NotFoundException` → 404 Not Found
- `DomainException` → 400 Bad Request
- Unhandled → 500 Internal Server Error

## ⚙️ Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=filoshop-db;Port=5432;...",
    "Cache": "filoshop-redis:6379"
  },
  "Authentication": {
    "Audience": "filoshop-api",
    "MetadataUrl": "http://keycloak:8080/..."
  },
  "Outbox": {
    "IntervalInSeconds": 10,
    "BatchSize": 20
  }
}
```

## 🔌 Dependency Injection

Configured in `Startup.cs`:

```csharp
services.AddApplication()             // Application layer
       .AddInfrastructurePersistence(config)  // Persistence
       .AddInfrastructureServices(config);    // Auth, Caching
```

## 🗺️ Endpoints

### CatalogItems
- `GET /api/v1/catalogitems` - List items (paginated)
- `GET /api/v1/catalogitems/{id}` - Get by ID
- `POST /api/v1/catalogitems` - Create item (idempotent)
- `PUT /api/v1/catalogitems/{id}` - Update item
- `DELETE /api/v1/catalogitems/{id}` - Delete item

### Users
- `POST /api/v1/users/register` - Register new user
- `GET /api/v1/users/{id}` - Get user details

### Orders
- `POST /api/v1/orders` - Create order (idempotent)
- `GET /api/v1/orders/{id}` - Get order details

### Baskets
- `GET /api/v1/baskets/{userId}` - Get user's basket
- `POST /api/v1/baskets/items` - Add item to basket
- `DELETE /api/v1/baskets/items/{id}` - Remove item

## 🛠️ Middleware Pipeline

1. **Exception Handling** - Global error handling
2. **Logging** - Serilog (outputs to Seq)
3. **Authentication** - JWT Bearer (Keycloak)
4. **Authorization** - Permission-based
5. **CORS** - Configured for frontend
6. **Swagger** - API documentation

## 🐳 Docker Support

Includes:
- Multi-stage `Dockerfile` for optimized builds
- `docker-compose.yaml` for full stack:
  - PostgreSQL
  - Redis
  - Keycloak
  - Seq (logging)

## 🔗 Dependencies

- **ASP.NET Core 9.0**
- **MediatR** - CQRS
- **FluentValidation.AspNetCore**
- **Swashbuckle** - Swagger/OpenAPI
- **Serilog** - Logging
- **Microsoft.AspNetCore.Authentication.JwtBearer**

## 🚀 Running Locally

```powershell
# Using Docker Compose
docker compose up --build

# Or locally
dotnet run --project FiloShop.Api
```

## 📝 API Documentation

Access Swagger UI at: `http://localhost:5000/swagger`

---

This layer is **stable** and primarily handles HTTP concerns. Business logic lives in Application/Domain layers.