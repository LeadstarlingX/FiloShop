using FiloShop.Domain.Baskets.Entities;
using FiloShop.Domain.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class BasketEntityConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.Property(b => b.UserId).IsRequired();

        // Navigation to BasketItems (child entities)
        builder.HasMany<BasketItem>()
            .WithOne()
            .HasForeignKey(bi => bi.BasketId);

        // Ignore calculated property
        builder.Ignore(b => b.TotalItems);
    }
}