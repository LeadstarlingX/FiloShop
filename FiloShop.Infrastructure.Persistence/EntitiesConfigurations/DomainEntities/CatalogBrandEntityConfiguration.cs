using FiloShop.Domain.CatalogBrands.Entities;
using FiloShop.Domain.CatalogBrands.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class CatalogBrandEntityConfiguration : IEntityTypeConfiguration<CatalogBrand>
{
    public void Configure(EntityTypeBuilder<CatalogBrand> builder)
    {
        // Brand Value Object
        builder.Property(cb => cb.Brand)
            .HasConversion(
                b => b.Value,
                value => new Brand(value))
            .IsRequired();
    }
}