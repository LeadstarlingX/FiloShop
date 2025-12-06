using FiloShop.Domain.Users.Entities;

namespace FiloShop.Infrastructure.Services.Authorization;

public class UserRolesResponse
{
    public Guid Id { get; init; }

    public List<Role> Roles { get; init; } = [];
}