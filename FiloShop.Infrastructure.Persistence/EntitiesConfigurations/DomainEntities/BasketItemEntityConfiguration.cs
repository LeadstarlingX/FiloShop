using FiloShop.Domain.Baskets.Entities;
using FiloShop.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class BasketItemEntityConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {

        // Money Value Object (UnitPrice)
        builder.ComplexProperty(bi => bi.UnitPrice, moneyBuilder =>
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

        builder.Property(bi => bi.Quantity).IsRequired();
        builder.Property(bi => bi.CatalogItemId).IsRequired();
        builder.Property(bi => bi.BasketId).IsRequired();
    }
}
