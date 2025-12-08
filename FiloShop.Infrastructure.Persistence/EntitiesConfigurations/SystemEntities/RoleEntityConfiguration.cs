using FiloShop.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.SystemEntities;

internal sealed class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany(role => role.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>();

        builder.HasData(Role.Registered);
    }
}