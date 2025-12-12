# FiloShop.Presentation

The **Presentation** layer contains controllers, view models, and API-related concerns for serving the application's HTTP interface. It acts as the interface between clients and the Application layer.

## 🎯 Responsibilities

- **Controllers** - HTTP endpoint handlers
- **DTOs/ViewModels** - Request/response models
- **API Contracts** - Public API shapes
- **Routing** - URL structure and versioning
- **Response Formatting** - Standardized API responses

## 📁 Structure

```
FiloShop.Presentation/
├── Controllers/
│   ├── CatalogItems/
│   ├── Users/
│   ├── Orders/
│   └── Baskets/
```

## 🎨 Key Patterns

### Controllers
Each aggregate has its own controller:
- `CatalogItemsController` - Product catalog management
- `UsersController` - User management and authentication
- `OrdersController` - Order processing
- `BasketsController` - Shopping cart operations

### Request/Response Flow
```
HTTP Request → Controller → MediatR Command/Query → Application Layer
                    ↓
              HTTP Response ← ApiResponse<T> ← Result<T>
```

### Standardized Responses
All endpoints return `ApiResponse<T>` from SharedKernel:

**Success**:
```json
{
  "data": { "id": "...", "name": "..." },
  "isSuccess": true,
  "error": null
}
```

**Failure**:
```json
{
  "data": null,
  "isSuccess": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input"
  }
}
```

### Idempotency Headers
Commands support idempotency via `X-Idempotency-Key` header:

```csharp
[HttpPost]
public async Task<IActionResult> CreateCatalogItem(
    [FromBody] CreateCatalogItemRequest request,
    [FromHeader(Name = "X-Idempotency-Key")] Guid? idempotencyKey)
{
    var command = request.ToCommand() with 
    { 
        IdempotencyKey = idempotencyKey ?? Guid.NewGuid() 
    };
    
    var result = await _sender.Send(command);
    return result.ToActionResult();
}
```

## 🔗 Dependencies

- **FiloShop.Application** - Commands and queries
- **FiloShop.SharedKernel** - Common types
- **ASP.NET Core MVC**
- **MediatR** - CQRS pattern

## 📝 Controller Conventions

### Naming
- **Plural** aggregate names (e.g., `CatalogItemsController`)
- **RESTful** routes following HTTP standards
- **Versioned** URLs (`/api/v1/...`)

### HTTP Verbs
- `GET` - Queries (no state change)
- `POST` - Create commands (idempotent)
- `PUT` - Update commands (idempotent)
- `DELETE` - Delete commands
- `PATCH` - Partial updates

### Response Codes
- `200 OK` - Successful GET/PUT/PATCH
- `201 Created` - Successful POST
- `204 No Content` - Successful DELETE
- `400 Bad Request` - Validation failure
- `404 Not Found` - Resource not found
- `409 Conflict` - Business rule violation
- `500 Internal Server Error` - Unhandled exceptions

## 🛡️ Security

- **JWT Authentication** - Keycloak integration
- **Permission-based Authorization** - Granular access control
- **CORS** - Configured for frontend origins

## 📚 Example Controller

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/catalogitems")]
public class CatalogItemsController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogItemsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalogItems(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetCatalogItemsQuery(pageNumber, pageSize);
        var result = await _sender.Send(query);
        return Ok(ApiResponse<PaginatedResult<CatalogItemDto>>.Success(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCatalogItem(Guid id)
    {
        var query = new GetCatalogItemByIdQuery(id);
        var result = await _sender.Send(query);
        
        return result.Match(
            success => Ok(ApiResponse<CatalogItemDto>.Success(success)),
            error => NotFound(ApiResponse<CatalogItemDto>.Failure(error))
        );
    }
}
```

## ⚠️ Important Notes

- **No business logic** - Controllers only coordinate
- **Thin layer** - Delegates everything to Application layer
- **Idempotency** - All mutation endpoints support it
- **Validation** - Happens in Application layer via FluentValidation

---

This layer is **stable** and focuses purely on HTTP concerns. Changes should only occur for API contract modifications.