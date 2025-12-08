# FiloShop.Domain

The **Domain** layer is the heart of the application, containing all business logic, domain entities, value objects, and domain events. It implements Domain-Driven Design (DDD) principles and is completely independent of infrastructure concerns.

## 🏛️ Architecture

This layer follows **DDD tactical patterns** with a focus on:
- **Aggregates** with clear boundaries
- **Value Objects** for domain concepts
- **Domain Events** for loose coupling
- **Repository Interfaces** (implementation in Infrastructure)

## 📦 Aggregates

### 👤 Users
**Aggregate Root**: `User`

Manages user identity, authentication, and payment methods.

**Entities**:
- `User` - Aggregate root
- `Role` - User roles with permissions
- `Permission` - Granular access control

**Value Objects**:
- `FirstName`, `LastName` - Personal information
- `Email` - Email address with validation
- `PaymentMethod` - Stored payment information

**Events**:
- `UserCreatedDomainEvent`

### 📦 Orders
**Aggregate Root**: `Order`

Represents customer purchase orders with line items and shipping details.

**Entities**:
- `Order` - Aggregate root
- `OrderItem` - Individual order line items

**Value Objects**:
- `Address` - Shipping address
- `CatalogItemOrdered` - Snapshot of ordered catalog item

**Events**:
- `OrderCreatedDomainEvent`

### 🛍️ CatalogItems
**Aggregate Root**: `CatalogItem`

Product catalog with brands and categories.

**Entities**:
- `CatalogItem` - Aggregate root

**Value Objects**:
- `CatalogItemDetails` - Product details
- Related to `CatalogBrand` and `CatalogType`

**Events**:
- `CatalogItemCreatedDomainEvent`

### 🛒 Baskets
**Aggregate Root**: `Basket`

Shopping cart functionality for users.

**Entities**:
- `Basket` - Aggregate root
- `BasketItem` - Items in cart

**Events**:
- `BasketCreatedDomainEvent`

### 🏷️ CatalogBrands
**Aggregate Root**: `CatalogBrand`

Product brands/manufacturers.

**Value Objects**:
- `Brand` - Brand name

**Events**:
- `CatalogBrandCreatedDomainEvent`

### 📋 CatalogTypes
**Aggregate Root**: `CatalogType`

Product categories/types.

**Value Objects**:
- `Type` - Category name

**Events**:
- `CatalogTypeCreatedDomainEvent`

## 🎨 Shared Value Objects

Located in `Shared/ValueObjects`:

- `Money` - Currency amount with validation
- `Currency` - ISO currency codes (USD, EUR, SYP)
- `Name` - String with length validation
- `Description` - Text description
- `Email` - Email with validation
- `PhoneNumber` - Contact number
- `Url` - Web address
- `ContactInfo` - Contact details

## 📁 Structure

```
FiloShop.Domain/
├── Users/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   ├── IRepository/
│   └── Errors/
├── Orders/
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   ├── IRepository/
│   └── Errors/
├── CatalogItems/
├── Baskets/
├── CatalogBrands/
├── CatalogTypes/
└── Shared/
    ├── ValueObjects/
    └── Errors/
```

## 🎯 Design Principles

1. **Aggregate Boundaries** - Each aggregate maintains its own consistency
2. **Value Objects** - Immutable, validated domain concepts
3. **Domain Events** - Loose coupling between aggregates
4. **Rich Domain Model** - Business logic lives in entities
5. **Persistence Ignorance** - No infrastructure dependencies

## 🔗 Dependencies

- **FiloShop.SharedKernel** - Base classes and interfaces only
- **No** infrastructure dependencies
- **No** external frameworks (except annotations)

## 📝 Entity Configuration

All entities use EF Core for persistence (configured in Infrastructure layer):

- Value Objects mapped with `ComplexProperty` and `HasConversion`
- Audit fields (`CreatedAt`, `UpdatedAt`) auto-populated via interceptor
- All column names match property names (no `_Value` suffix)

## 🚀 Usage Example

```csharp
// Create an order (rich domain model)
var order = Order.Create(user, shippingAddress);

// Domain event is raised automatically
// OrderCreatedDomainEvent will be published via Outbox

// Add items through methods (not direct property access)
order.AddItem(catalogItem, quantity);
```

## ⚠️ Important Notes

- **Repositories are interfaces** - Implemented in `Infrastructure.Persistence`
- **Domain Events** - Published via Outbox pattern
- **Value Objects** - Always use factory methods (e.g., `Money.Create()`)
- **Validation** - Happens in value object constructors and entity methods

---

This layer is **stable** and should only change when business requirements evolve.