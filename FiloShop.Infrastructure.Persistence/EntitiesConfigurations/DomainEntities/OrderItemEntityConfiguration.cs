using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Money Value Object (UnitPrice)
        builder.ComplexProperty(oi => oi.UnitPrice, moneyBuilder =>
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

        // CatalogItemOrdered Value Object
        builder.ComplexProperty(oi => oi.CatalogItemOrdered, catalogBuilder =>
        {
            catalogBuilder.Property(c => c.CatalogItemId).IsRequired();
            
            catalogBuilder.Property(c => c.ProductName)
                .HasConversion(
                    n => n.Value,
                    value => Name.Create(value).Value)
                .IsRequired();
            
            catalogBuilder.Property(c => c.PictureUrl)
                .HasConversion(
                    u => u.Value,
                    value => Url.Create(value).Value)
                .IsRequired(false);
        });

        builder.Property(oi => oi.Units).IsRequired();
    }
}
