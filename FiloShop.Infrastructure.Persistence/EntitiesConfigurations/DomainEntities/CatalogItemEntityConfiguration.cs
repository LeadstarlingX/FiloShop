using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class CatalogItemEntityConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
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

        // Money Value Object (Price)
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