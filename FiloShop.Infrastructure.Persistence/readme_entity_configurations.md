# Entity Configurations Guide

This guide covers **EF Core entity configurations** for mapping domain entities to the PostgreSQL database using the Fluent API.

## 📖 Overview

Entity configurations define how domain entities are stored in the database:
- Table and column names
- Primary and foreign keys
- Value object mappings
- Relationships
- Indexes and constraints

## 📁 File Structure

```
FiloShop.Infrastructure.Persistence/
└── EntitiesConfigurations/
    ├── DomainEntities/
    │   ├── UserEntityConfiguration.cs
    │   ├── OrderEntityConfiguration.cs
    │   ├── OrderItemEntityConfiguration.cs
    │   ├── CatalogItemEntityConfiguration.cs
    │   ├── BasketEntityConfiguration.cs
    │   ├── BasketItemEntityConfiguration.cs
    │   ├── CatalogBrandEntityConfiguration.cs
    │   └── CatalogTypeEntityConfiguration.cs
    └── SystemEntities/
        ├── OutboxMessageConfiguration.cs
        └── IdempotencyRecordConfiguration.cs
```

## 🔧 Basic Configuration

### Template

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primary key
        builder.HasKey(o => o.Id);

        // Required properties
        builder.Property(o => o.UserId).IsRequired();

        // Value objects
        builder.ComplexProperty(o => o.ShipToAddress, ...);

        // Relationships
        builder.HasMany<OrderItem>()...
    }
}
```

## 🎨 Value Object Mapping

### ComplexProperty (EF Core 8+)

For value objects that are part of the same table:

```csharp
// Address Value Object
builder.ComplexProperty(o => o.ShipToAddress, addressBuilder =>
{
    addressBuilder.Property(a => a.Country).IsRequired();
    addressBuilder.Property(a => a.Street).IsRequired();
    addressBuilder.Property(a => a.City).IsRequired();
    addressBuilder.Property(a => a.State).IsRequired();
    addressBuilder.Property(a => a.ZipCode).IsRequired();
});

// Generated columns: ShipToAddress_Country, ShipToAddress_Street, etc.
```

### HasConversion

For simple value objects:

```csharp
// Email Value Object
builder.Property(u => u.Email)
    .HasConversion(
        e => e.Value,                        // To database
        value => Email.Create(value).Value)  // From database
    .IsRequired();

// Generated column: Email (string)
```

### Complex Value Objects with Currency

```csharp
// Money Value Object
builder.ComplexProperty(oi => oi.UnitPrice, moneyBuilder =>
{
    moneyBuilder.Property(m => m.Amount)
        .HasPrecision(18, 2)
        .IsRequired();

    moneyBuilder.Property(m => m.Currency)
        .HasConversion(
            c => c.Code,                          // Store "USD"
            code => Currency.FromCode(code).Value)  // Retrieve Currency object
        .IsRequired();
});

// Generated columns: UnitPrice_Amount, UnitPrice_Currency
```

## 🔗 Relationship Mapping

### One-to-Many

```csharp
// Order has many OrderItems
builder.HasMany<OrderItem>()
    .WithOne()
    .HasForeignKey("OrderId");

// Basket has many BasketItems
builder.HasMany<BasketItem>()
    .WithOne()
    .HasForeignKey(bi => bi.BasketId);
```

### Many-to-Many

```csharp
// User has many Roles (and vice versa)
builder.HasMany(u => u.Roles)
    .WithMany(r => r.Users)
    .UsingEntity(joinBuilder =>
    {
        joinBuilder.ToTable("UserRoles");
    });

// Generated: UserRoles table with UsersId and RolesId
```

### Owned Entities (OwnsMany)

```csharp
// User owns many PaymentMethods
builder.OwnsMany(u => u.PaymentMethods, paymentBuilder =>
{
    paymentBuilder.Property(pm => pm.Alias).IsRequired(false);
    paymentBuilder.Property(pm => pm.CardId).IsRequired(false);
    paymentBuilder.Property(pm => pm.Last4).IsRequired(false);
});

// PaymentMethod table created with UserId foreign key
```

## 📋 Real Examples

### UserEntityConfiguration

```csharp
public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary Key (inherited from BaseEntity)
        builder.HasKey(u => u.Id);

        // FirstName Value Object
        builder.Property(u => u.FirstName)
            .HasConversion(
                fn => fn.Value,
                value => FirstName.Create(value).Value)
            .IsRequired();

        // LastName Value Object
        builder.Property(u => u.LastName)
            .HasConversion(
                ln => ln.Value,
                value => LastName.Create(value).Value)
            .IsRequired();

        // Email Value Object
        builder.Property(u => u.Email)
            .HasConversion(
                e => e.Value,
                value => Email.Create(value).Value)
            .IsRequired();

        builder.Property(u => u.IdentityId).IsRequired(false);
        builder.Property(u => u.IsActive).IsRequired();

        // PaymentMethods - Owned Collection
        builder.OwnsMany(u => u.PaymentMethods, paymentBuilder =>
        {
            paymentBuilder.Property(pm => pm.Alias).IsRequired(false);
            paymentBuilder.Property(pm => pm.CardId).IsRequired(false);
            paymentBuilder.Property(pm => pm.Last4).IsRequired(false);
        });

        // Roles - Many-to-Many
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("UserRoles");
            });
    }
}
```

### CatalogItemEntityConfiguration

```csharp
public class CatalogItemEntityConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        // Name Value Object
        builder.Property(ci => ci.Name)
            .HasConversion(
                n => n.Value,
                value => Name.Create(value).Value)
            .IsRequired();

        // Description Value Object
        builder.Property(ci => ci.Description)
            .HasConversion(
                d => d.Value,
                value => Description.Create(value).Value)
            .IsRequired();

        // Money Value Object (Nested Currency)
        builder.ComplexProperty(ci => ci.Price, moneyBuilder =>
        {
            moneyBuilder.Property(m => m.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            moneyBuilder.Property(m => m.Currency)
                .HasConversion(
                    c => c.Code,
                    code => Currency.FromCode(code).Value)
                .IsRequired();
        });

        // Url Value Object
        builder.Property(ci => ci.PictureUri)
            .HasConversion(
                u => u.Value,
                value => Url.Create(value).Value)
            .IsRequired(false);

        builder.Property(ci => ci.CatalogTypeId).IsRequired();
        builder.Property(ci => ci.CatalogBrandId).IsRequired();
    }
}
```

## ⚙️ Configuration Registration

Configurations are auto-discovered in `ApplicationDbContext`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Apply all configurations in this assembly
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
}
```

## 🎯 Common Patterns

### Pattern 1: BaseEntity Properties

**Don't** configure globally:
```csharp
// ❌ Not needed - inheritance handles it
builder.Property(e => e.Id);
builder.Property(e => e.CreatedAt);
builder.Property(e => e.UpdatedAt);
```

These are handled by `BaseEntity` and the `AuditableEntityInterceptor`.

### Pattern 2: Precision for Decimals

```csharp
// ✅ Always specify precision for money
moneyBuilder.Property(m => m.Amount)
    .HasPrecision(18, 2);  // 18 total digits, 2 after decimal
```

### Pattern 3: Nullable vs Required

```csharp
// Required
builder.Property(o => o.UserId).IsRequired();

// Optional
builder.Property(u => u.IdentityId).IsRequired(false);
```

### Pattern 4: Indexes

```csharp
// Single column index
builder.HasIndex(u => u.Email).IsUnique();

// Composite index
builder.HasIndex(o => new { o.UserId, o.OrderDate });

// Filtered index
builder.HasIndex(o => o.ProcessedOnUtc)
    .HasFilter("\"ProcessedOnUtc\" IS NULL");
```

## ✅ Best Practices

### 1. No Table/Column Names in Configurations

FiloShop convention: Don't specify names (use defaults):

```csharp
// ❌ Don't do this
builder.ToTable("Orders");
builder.Property(o => o.UserId).HasColumnName("user_id");

// ✅ Do this - let EF use property names
builder.HasKey(o => o.Id);
builder.Property(o => o.UserId).IsRequired();
```

**Result**: Table `Order`, column `UserId` (matches C# names).

### 2. Value Objects Don't Get Separate Tables

```csharp
// ✅ ComplexProperty - same table
builder.ComplexProperty(o => o.ShipToAddress, ...);

// ✅ HasConversion - same table
builder.Property(u => u.Email)
    .HasConversion(...);

// ❌ OwnsOne - creates separate table (avoid unless needed)
```

### 3. Explicit Foreign Keys

```csharp
// ✅ Good - explicit
builder.HasMany<OrderItem>()
    .WithOne()
    .HasForeignKey(oi => oi.OrderId);

// ❌ Bad - implicit (less clear)
builder.HasMany<OrderItem>()
    .WithOne();
```

### 4. Complex Property Naming

EF Core generates column names like `ValueObject_Property`:

```csharp
builder.ComplexProperty(o => o.ShipToAddress, addressBuilder =>
{
    addressBuilder.Property(a => a.Street).IsRequired();
});

// Generated column: ShipToAddress_Street
```

## 🔍 Verifying Configurations

### Check Generated SQL

After creating a migration:

```powershell
dotnet ef migrations add TestMigration
```

Review the generated `Up` method to verify:
- Column types
- Constraints
- Indexes
- Foreign keys

## ⚠️ Common Mistakes

### ❌ Forgetting .Value for Value Objects

```csharp
// Wrong
builder.Property(u => u.Email)
    .HasConversion(
        e => e,  // ❌ Should be e.Value
        value => Email.Create(value).Value);
```

### ❌ Missing Precision for Decimals

```csharp
// Wrong
builder.Property(m => m.Amount);  // ❌ No precision

// Right
builder.Property(m => m.Amount)
    .HasPrecision(18, 2);  // ✅
```

### ❌ Configuring BaseEntity Properties

```csharp
// Wrong - BaseEntity handles these
builder.Property(e => e.Id).HasKey();  // ❌
builder.Property(e => e.CreatedAt);    // ❌
```

## 📊 Migration Result

After configuration and migration, your database schema will look like:

```sql
CREATE TABLE "User" (
    "Id" UUID PRIMARY KEY,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "IdentityId" TEXT,
    "IsActive" BOOLEAN NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL,
    "UpdatedAt" TIMESTAMP
);

CREATE TABLE "PaymentMethod" (
    "Id" INT PRIMARY KEY,
    "UserId" UUID NOT NULL,
    "Alias" TEXT,
    "CardId" TEXT,
    "Last4" TEXT,
    FOREIGN KEY ("UserId") REFERENCES "User"("Id")
);
```

---

Entity configurations bridge your **rich domain model** with **database storage**, keeping persistence concerns out of the domain layer.
