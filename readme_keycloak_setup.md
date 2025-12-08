# Keycloak Setup & Integration Guide

This guide covers **Keycloak** setup for FiloShop, including installation, configuration, realm setup, and JWT authentication integration.

## 📖 Overview

**Keycloak** is an open-source Identity and Access Management (IAM) solution providing:
- User authentication (login/logout)
- JWT token generation
- Role-based access control (RBAC)
- OAuth 2.0 / OpenID Connect
- User management UI

## 🐳 Docker Setup

### docker-compose.yaml

```yaml
services:
  keycloak:
    image: quay.io/keycloak/keycloak:latest
    container_name: filoshop-keycloak
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin
    ports:
      - "8080:8080"
    command:
      - start-dev
    networks:
      - filoshop-network
```

### Start Keycloak

```powershell
docker compose up -d keycloak
```

Access admin console: **http://localhost:8080**

**Credentials**: `admin` / `admin`

## ⚙️ Realm Configuration

### 1. Create Realm

1. Click **Create Realm**
2. Name: `filoshop`
3. Click **Create**

### 2. Create Client

1. Navigate to **Clients**
2. Click **Create Client**
3. Settings:
   - **Client ID**: `filoshop-api`
   - **Client authentication**: OFF
   - **Standard flow**: ON
   - **Direct access grants**: ON
4. **Save**

### 3. Configure Client Settings

In the `filoshop-api` client:

**Access Settings**:
```
Root URL: http://localhost:5000
Valid Redirect URIs: http://localhost:5000/*
Web Origins: http://localhost:5000
```

**Capability Config**:
- Client authentication: OFF (public client)
- Authorization: OFF
- Authentication flow:
  - ✅ Standard flow
  - ✅ Direct access grants
  - ❌ Implicit flow (deprecated)

### 4. Create Roles

Navigate to **Realm Roles**:

1. Click **Create Role**
2. Add roles:
   - `user` - Regular user
   - `admin` - Administrator
   - `moderator` - Content moderator

### 5. Create Users

Navigate to **Users** → **Add User**:

**User 1 (Admin)**:
```
Username: admin@filoshop.com
Email: admin@filoshop.com
Email Verified: ON
```

After creation:
1. **Credentials** tab → Set password → Turn off "Temporary"
2. **Role Mapping** → Assign `admin` role

**User 2 (Regular User)**:
```
Username: user@filoshop.com
Email: user@filoshop.com
Email Verified: ON
```

Assign `user` role.

## 🔧 Application Configuration

### appsettings.json

```json
{
  "Authentication": {
    "Schemes": {
      "Bearer": {
        "ValidAudiences": [
          "filoshop-api"
        ],
        "ValidIssuer": "http://localhost:8080/realms/filoshop"
      }
    },
    "MetadataAddress": "http://localhost:8080/realms/filoshop/.well-known/openid-configuration"
  },
  "Keycloak": {
    "AdminUrl": "http://localhost:8080/admin/realms/filoshop",
    "TokenUrl": "http://localhost:8080/realms/filoshop/protocol/openid-connect"
  }
}
```

### AuthenticationOptions.cs

```csharp
public class AuthenticationOptions
{
    public string MetadataAddress { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
}
```

### KeycloakOptions.cs

```csharp
public class KeycloakOptions
{
    public string AdminUrl { get; init; } = string.Empty;
    public string TokenUrl { get; init; } = string.Empty;
}
```

## 🔐 JWT Authentication Setup

### DependencyInjection.cs

```csharp
private static IServiceCollection AddMyAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer();

    services.Configure<AuthenticationOptions>(
        configuration.GetSection("Authentication"));

    services.ConfigureOptions<JwtBearerOptionsSetup>();

    services.Configure<KeycloakOptions>(
        configuration.GetSection("Keycloak"));

    return services;
}
```

### JwtBearerOptionsSetup.cs

```csharp
public class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions;

    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        options.MetadataAddress = _authenticationOptions.MetadataAddress;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = "http://localhost:8080/realms/filoshop",
            ValidAudience = "filoshop-api",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    }

    public void Configure(JwtBearerOptions options) => Configure(string.Empty, options);
}
```

## 🎯 Using Authentication

### Protect Endpoints

```csharp
[ApiController]
[Route("api/v{version:apiVersion}/orders")]
[Authorize]  // ← Requires authentication
public class OrdersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        // Authenticated users only
    }

    [HttpPost]
    [Authorize(Roles = "admin")]  // ← Requires admin role
    public async Task<IActionResult> CreateOrder(...)
    {
        // Admins only
    }
}
```

### Get Current User

Create `UserContext`:

```csharp
public interface IUserContext
{
    Guid UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
}

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => GetClaimValue<Guid>(ClaimTypes.NameIdentifier);

    public string Email => GetClaimValue<string>(ClaimTypes.Email) ?? string.Empty;

    public bool IsAuthenticated => 
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private T? GetClaimValue<T>(string claimType)
    {
        var claim = _httpContextAccessor.HttpContext?.User
            .FindFirst(claimType)?.Value;

        return claim != null ? (T)Convert.ChangeType(claim, typeof(T)) : default;
    }
}
```

Register in DI:

```csharp
services.AddHttpContextAccessor();
services.AddScoped<IUserContext, UserContext>();
```

## 🔑 Getting Tokens

### Using Postman

**Request**:
```
POST http://localhost:8080/realms/filoshop/protocol/openid-connect/token
Content-Type: application/x-www-form-urlencoded

Body (x-www-form-urlencoded):
grant_type: password
client_id: filoshop-api
username: admin@filoshop.com
password: yourpassword
```

**Response**:
```json
{
  "access_token": "eyJhbGciOi...",
  "expires_in": 300,
  "refresh_token": "eyJhbGciOi...",
  "token_type": "Bearer"
}
```

### Programmatic Token Request

```csharp
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;

    public async Task<string> GetAccessTokenAsync(string username, string password)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "filoshop-api",
                ["username"] = username,
                ["password"] = password
            })
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);

        return tokenResponse!.AccessToken;
    }
}
```

## 📝 Testing with JWT

### In Swagger

1. Click **Authorize** button
2. Enter: `Bearer {your-token}`
3. Click **Authorize**

### In Postman

**Headers**:
```
Authorization: Bearer eyJhbGciOi...
```

### In Code (Integration Tests)

```csharp
public async Task<HttpClient> GetAuthenticatedClientAsync()
{
    var client = _factory.CreateClient();
    
    var token = await GetTokenAsync("admin@filoshop.com", "password");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);

    return client;
}
```

## 🎨 Custom Claims

### Add Custom Claims in Keycloak

1. **Client Scopes** → **filoshop-api** → **Mappers**
2. Click **Create**
3. Settings:
   - Mapper Type: User Attribute
   - Name: `user_id`
   - User Attribute: `id`
   - Token Claim Name: `sub`
   - Claim JSON Type: String

### Read Custom Claims

```csharp
public Guid UserId => GetClaimValue<Guid>("sub");
public string Email => GetClaimValue<string>("email");
public List<string> Roles => GetClaimValues("realm_roles");
```

## 🛡️ Permission-Based Authorization

### Custom Authorization Handler

```csharp
public class PermissionAuthorizationHandler 
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context.User
            .FindAll("permissions")
            .Select(c => c.Value)
            .ToList();

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
```

### Usage

```csharp
[Authorize(Policy = "users:read")]
public async Task<IActionResult> GetUsers()
{
    // Only users with "users:read" permission
}
```

## ⚙️ Production Configuration

### Environment Variables

```yaml
# docker-compose.prod.yaml
services:
  keycloak:
    environment:
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: ${KEYCLOAK_DB_PASSWORD}
      KC_HOSTNAME: auth.filoshop.com
      KC_HTTPS_CERTIFICATE_FILE: /opt/keycloak/conf/cert.pem
      KC_HTTPS_CERTIFICATE_KEY_FILE: /opt/keycloak/conf/key.pem
    command:
      - start
      - --optimized
```

### Use PostgreSQL Backend

Keycloak should use PostgreSQL (not H2) in production:

```yaml
KC_DB: postgres
KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak
```

## ✅ Best Practices

1. **Use HTTPS** in production
2. **Rotate secrets** regularly
3. **Enable MFA** for admin accounts
4. **Limit token lifetime** (5-15 minutes)
5. **Use refresh tokens** for long sessions
6. **Store tokens securely** (HttpOnly cookies)
7. **Validate tokens** on every request
8. **Log authentication events**

## 🔍 Troubleshooting

### Token Validation Fails

**Error**: `401 Unauthorized`

**Check**:
1. Token hasn't expired
2. `ValidIssuer` matches Keycloak realm
3. `ValidAudience` matches client ID
4. Keycloak is accessible from API

### Claims Not Available

**Check**:
1. Client scopes configured
2. Mappers added
3. Token includes expected claims (decode at jwt.io)

### CORS Errors

Add to Keycloak client:
```
Web Origins: http://localhost:5000
```

---

Keycloak provides **enterprise-grade authentication** with minimal configuration. Integrate once, secure everywhere!
