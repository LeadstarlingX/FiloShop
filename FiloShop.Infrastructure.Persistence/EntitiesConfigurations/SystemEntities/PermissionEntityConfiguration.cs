using FiloShop.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiloShop.Infrastructure.Persistence.EntitiesConfigurations.SystemEntities;

internal sealed class PermissionEntityConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasData(Permission.UsersRead);
    }
}