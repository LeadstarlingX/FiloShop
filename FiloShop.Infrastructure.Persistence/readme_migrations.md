# Database Migrations Guide

This guide covers creating, managing, and applying **EF Core migrations** for FiloShop's PostgreSQL database.

## 📖 Overview

### What are Migrations?

Migrations are version-controlled database schema changes that:
- Track schema evolution over time
- Enable team collaboration
- Support rollbacks
- Generate SQL scripts for production

### Migration Workflow

```
Code Changes → Add Migration → Review SQL → Apply to DB → Commit to Git
```

## 🛠️ Prerequisites

- .NET 9.0 SDK
- EF Core tools installed globally
- PostgreSQL running (via Docker or locally)

### Install EF Core Tools

```powershell
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef  # Update to latest
```

## 🚀 Creating Migrations

### Basic Command

```powershell
dotnet ef migrations add MigrationName \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

**Parameters**:
- `-p` (--project) - Where migrations are stored
- `-s` (--startup-project) - Where `Program.cs` and connection string are
- `MigrationName` - Descriptive name (PascalCase)

### Naming Conventions

✅ **Good Names**:
- `InitialCreate`
- `AddCatalogItemPricing`
- `UpdateUserEmailIndex`
- `AddOrderShippingAddress`

❌ **Bad Names**:
- `Migration1`
- `Updates`
- `FixStuff`
- `Test`

### Example: Adding a New Entity

```powershell
# 1. Create the entity in Domain layer
# 2. Add entity configuration in Infrastructure.Persistence
# 3. Create migration
dotnet ef migrations add AddWishlistEntity \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api

Success! Migration created: 20240109123456_AddWishlistEntity.cs
```

### From Rider IDE

```powershell
# Right-click on project → Tools → Entity Framework Core →
Add Migration
```

Or use the terminal in Rider with the same commands.

## 📁 Migration Files

Migrations are stored in:
```
FiloShop.Infrastructure.Persistence/
└── Migrations/
    ├── 20240109123456_AddWishlistEntity.cs
    ├── 20240109123456_AddWishlistEntity.Designer.cs
    └── ApplicationDbContextModelSnapshot.cs
```

### Migration Structure

```csharp
public partial class AddWishlistEntity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Wishlist",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Wishlist", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Wishlist");
    }
}
```

## 🔄 Applying Migrations

### Apply All Pending Migrations

```powershell
dotnet ef database update \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

### Apply Specific Migration

```powershell
# Apply up to a specific migration
dotnet ef database update 20240109123456_AddWishlistEntity \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

### Rollback to Previous Migration

```powershell
# Rollback last migration
dotnet ef database update PreviousMigrationName \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api

# Rollback all migrations
dotnet ef database update 0 \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

## 🐳 Docker Environment

### Update Connection String

For local migrations, temporarily update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=filoshop;User Id=postgres;Password=postgres"
  }
}
```

**Note**: Port `5433` because Docker maps PostgreSQL to this port.

### Or Use Design-Time Factory

Create `ApplicationDbContextFactory.cs` to bypass connection string issues:

```csharp
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5433;Database=filoshop;User Id=postgres;Password=postgres");

        return new ApplicationDbContext(
            optionsBuilder.Options, 
            new SystemDateTimeProvider());
    }
}
```

## 📜 Generate SQL Scripts

### For Production Deployment

```powershell
# Generate SQL for all migrations
dotnet ef migrations script \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api \
  -o migration.sql

# Generate SQL from specific migration
dotnet ef migrations script FromMigration ToMigration \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api \
  -o incremental.sql
```

### Idempotent Scripts

Generate scripts that can be run multiple times safely:

```powershell
dotnet ef migrations script \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api \
  -o migration.sql \
  --idempotent
```

## 🔍 Reviewing Migrations

### View Migration SQL

```powershell
dotnet ef migrations script \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

### Check Pending Migrations

```csharp
// In code
var pendingMigrations = await dbContext.Database
    .GetPendingMigrationsAsync();

if (pendingMigrations.Any())
{
    Console.WriteLine("Pending migrations:");
    foreach (var migration in pendingMigrations)
    {
        Console.WriteLine($"  - {migration}");
    }
}
```

### List All Migrations

```powershell
dotnet ef migrations list \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

## 🗑️ Removing Migrations

### Remove Last Migration (Not Applied)

```powershell
dotnet ef migrations remove \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

⚠️ **Only works if not applied to database yet!**

### Remove Applied Migration

1. Rollback the database first
2. Then remove the migration file

```powershell
# 1. Rollback
dotnet ef database update PreviousMigrationName \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api

# 2. Remove
dotnet ef migrations remove \
  -p FiloShop.Infrastructure.Persistence \
  -s FiloShop.Api
```

## 🎯 Automatic Migrations at Startup

In `ApplicationBuilderExtensions.cs`:

```csharp
public static void ApplyMigrations(this IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.CreateScope();
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    
    // Apply all pending migrations
    dbContext.Database.Migrate();
}
```

In `Startup.cs`:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.ApplyMigrations();  // ← Run migrations on startup
    app.SeedData();         // ← Then seed data
    // ...
}
```

## ✅ Best Practices

### 1. Review Before Applying
Always review generated SQL before applying to production.

### 2. Backup Before Migration
```sql
pg_dump filoshop > backup_$(date +%Y%m%d).sql
```

### 3. Test Migrations
- Apply to dev/staging first
- Test rollback scenarios
- Verify data integrity

### 4. Keep Migrations Small
- One logical change per migration
- Easier to review and rollback

### 5. Never Edit Applied Migrations
- Create a new migration instead
- Maintain migration history integrity

### 6. Name Descriptively
```powershell
# ✅ Good
dotnet ef migrations add AddUserEmailUniqueIndex

# ❌ Bad
dotnet ef migrations add Update2
```

### 7. Use Data Migrations Carefully
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Schema change
    migrationBuilder.AddColumn<string>("Status", "Orders", nullable: false);

    // Data migration
    migrationBuilder.Sql(@"
        UPDATE ""Orders"" 
        SET ""Status"" = 'Pending' 
        WHERE ""Status"" IS NULL;
    ");
}
```

## ⚠️ Common Issues

### Issue: "No DbContext was found"

**Solution**: Specify startup project explicitly:
```powershell
dotnet ef migrations add MyMigration -s FiloShop.Api
```

### Issue: "Unable to create DbContext"

**Solution**: Ensure connection string is correct and PostgreSQL is running:
```powershell
docker compose ps  # Check if postgres is up
```

### Issue: "Target content not found"

**Solution**: Migrations out of sync. Remove and recreate:
```powershell
dotnet ef migrations remove
dotnet ef migrations add RecreatedMigration
```

## 📊 Migration in CI/CD

### GitHub Actions Example

```yaml
- name: Apply Migrations
  run: |
    dotnet ef database update \
      -p FiloShop.Infrastructure.Persistence \
      -s FiloShop.Api \
      --connection "${{ secrets.DB_CONNECTION_STRING }}"
```

### Docker Compose

Migrations run automatically on startup via `ApplyMigrations()`.

## 🔐 Production Deployment

### Option 1: Automatic (Startup)
Migrations run when application starts (current approach).

**Pros**: Simple, automatic  
**Cons**: Downtime during migration

### Option 2: Manual SQL Scripts
Generate and apply SQL manually:

```powershell
# 1. Generate SQL
dotnet ef migrations script -o prod_migration.sql --idempotent

# 2. Review SQL
cat prod_migration.sql

# 3. Apply to production
psql -U postgres -d filoshop -f prod_migration.sql
```

**Pros**: Full control, can review  
**Cons**: Manual process

### Option 3: CI/CD Pipeline
Run migrations in deployment pipeline before deploying app.

---

Migrations are a **critical part of database evolution**. Always review, test, and backup before applying to production!
