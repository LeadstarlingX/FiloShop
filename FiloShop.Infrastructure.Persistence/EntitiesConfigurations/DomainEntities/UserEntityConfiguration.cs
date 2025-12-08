using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.Domain.Users.Entities;
using FiloShop.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.DomainEntities;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
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

        // Roles - Many-to-Many with explicit join table
        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("UserRoles");
            });
    }
}