using FiloShop.Domain.Users.IRepository;
using FiloShop.SharedKernel.Interfaces;

namespace FiloShop.Infrastructure.Services.Authorization;

internal sealed class AuthorizationService
{
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _userRepository;

    public AuthorizationService(IUserRepository userRepository, ICacheService cache)
    {
        _userRepository = userRepository;
        _cacheService = cache;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";

        var cachedRoles = await _cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if (cachedRoles is not null) return cachedRoles;

        var user = await _userRepository.GetByIdentityIdAsync(identityId);

        var roles = new UserRolesResponse
        {
            Id = user!.Id,
            Roles = user.Roles.ToList()
        };

        await _cacheService.SetAsync(cacheKey, roles);

        return roles;
    }

    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";

        var cachedRoles = await _cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedRoles is not null) return cachedRoles;

        var permissionsSet = await _userRepository.GetPermissionsAsync(identityId);

        await _cacheService.SetAsync(cacheKey, permissionsSet);

        return permissionsSet;
    }
}