# Entity Configurations Guide

This guide ensures all developers add database mappings consistently using EF Core's Fluent API.

> [!IMPORTANT]
> **Never** use Data Annotation attributes (`[Key]`, `[Required]`, `[Table]`) in the Domain layer. All configuration must be done here.

## 📝 How to Add a New Configuration

1.  **Create File**: `FiloShop.Infrastructure.Persistence/EntitiesConfigurations/[BoundedContext]/[Entity]Configuration.cs`
2.  **Implement**: `IEntityTypeConfiguration<T>`
3.  **Naming Rules**:
    - **No Table/Column Names**: Let EF Core convention handle it (defaults to property names).
    - **Use Value Objects**: Always map value objects using `ComplexProperty` or `HasConversion`.

## 📌 Standard Patterns

### 1. Value Object (Single Property)

Use `HasConversion` for simple wrappers like `Email`, `Name`, `Id`.

```csharp
builder.Property(x => x.Email)
    .HasConversion(x => x.Value, v => Email.Create(v).Value)
    .IsRequired()
    .HasMaxLength(255);
```

### 2. Value Object (Multi-Column)

Use `ComplexProperty` (EF Core 8+) for objects like `Address`, `Money`.

```csharp
builder.ComplexProperty(o => o.Price, priceBuilder =>
{
    priceBuilder.Property(p => p.Amount).HasPrecision(18, 2);
    priceBuilder.Property(p => p.Currency).HasConversion(...);
});
```

### 3. Relationships

Always act on the "Many" side to keep configuration clear.

```csharp
// In OrderConfiguration
builder.HasMany(o => o.OrderItems)
    .WithOne()
    .HasForeignKey("OrderId") // Shadow property
    .IsRequired();
```

### 4. Audit Fields

**Do NOT** configure `CreatedAt` or `UpdatedAt` manually.
*   They are handled by `BaseEntity` configuration.
*   They are populated by `AuditableEntityInterceptor`.

## 🚫 Common Mistakes

*   ❌ naming columns manualy (`.HasColumnName("user_email")`) -> **Don't do it.** consistent C# naming is fine.
*   ❌ Forgetting `.IsRequired()` -> defaults to nullable strings.
*   ❌ Using `OwnsOne` instead of `ComplexProperty` (unless you need a separate table).
