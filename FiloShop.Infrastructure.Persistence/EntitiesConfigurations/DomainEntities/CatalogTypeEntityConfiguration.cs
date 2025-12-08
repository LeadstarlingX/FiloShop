using FiloShop.Domain.CatalogTypes.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Type = FiloShop.Domain.CatalogTypes.ValueObjects.Type;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class CatalogTypeEntityConfiguration : IEntityTypeConfiguration<CatalogType>
{
    public void Configure(EntityTypeBuilder<CatalogType> builder)
    {
        // Type Value Object
        builder.Property(ct => ct.Type)
            .HasConversion(
                t => t.Value,
                value => new Type(value))
            .IsRequired();
    }
}