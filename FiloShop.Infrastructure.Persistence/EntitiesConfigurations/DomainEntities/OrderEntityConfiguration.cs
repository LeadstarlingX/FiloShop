using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.Orders.Entities;
using FiloShop.Domain.Orders.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();

        // Address Value Object
        builder.ComplexProperty(o => o.ShipToAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Country).IsRequired();
            addressBuilder.Property(a => a.Street).IsRequired();
            addressBuilder.Property(a => a.City).IsRequired();
            addressBuilder.Property(a => a.State).IsRequired();
            addressBuilder.Property(a => a.ZipCode).IsRequired();
        });

        // Navigation to OrderItems (child entities)
        builder.HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey(o => o.OrderId);
    }
}