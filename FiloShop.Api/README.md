# FiloShop.API

## 🎯 Architecture & Responsibilities
The **API** layer is the entry point for the system. It is a thin translation layer that maps HTTP requests to Application Commands/Queries.

**Key Design Decision**: Controllers contain **zero business logic**. Their only job is to:
1.  Accept HTTP Request
2.  Dispatch specific Command/Query via MediatR
3.  Map Result to HTTP Status Code (200 OK, 400 Bad Request, etc.)

## 📂 Structure

Controllers are organized by feature, with a strict `Requests` folder for input contracts (DTOs).

```
FiloShop.API/
├── Controllers/            # (Reference to FiloShop.Presentation)
│   ├── Users/
│   │   ├── Requests/
│   │   │   ├── RegisterUserRequest.cs
│   │   │   └── LoginRequest.cs
│   │   └── UsersController.cs
│   └── ...
```

## 🔌 Middleware Pipeline
The implementation uses a standard ASP.NET Core pipeline enriched with system-wide concerns:

1.  **Global Exception Handler**: Converts exceptions to standard ProblemDetails JSON.
2.  **Serilog Request Logging**: Logs high-level request details (Method, Path, Duration) to Seq.
3.  **Authentication**: Validates JWT/Bearer tokens against Keycloak.
4.  **Authorization**: Checks user permissions/roles.
5.  **Swagger**: Generates OpenAPI definitions for the frontend.

## 🗺️ Key Features

### Standardized Responses
All API endpoints return a consistent envelope or ProblemDetails structure, ensuring the frontend always knows how to parse success vs. failure.

### Idempotency Support
The API middleware automatically detects the `X-Idempotency-Key` header on POST requests.
- If present, it enables the `IdempotentCommandBehavior`.
- If a duplicate key is sent, the API returns the *original* response (200/201) without processing the command again.

### API Versioning
We use URL-based versioning (`/api/v1/...`) to allow future breaking changes without disrupting existing clients.

## 🐳 Docker Support
The solution includes a full Docker stack:
- **API**: The .NET 9 application
- **Database**: PostgreSQL 16
- **Cache**: Redis 8
- **Identity**: Keycloak 26
- **Logging**: Seq

To run the full stack:
```powershell
docker compose up -d --build
```
Access Swagger at: `http://localhost:5000/swagger`