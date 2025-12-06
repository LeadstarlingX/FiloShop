using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FiloShop.Infrastructure.Services.Authorization;

internal sealed class PermissionsAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptionsptions;

    public PermissionsAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _authorizationOptionsptions = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null) return policy;

        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _authorizationOptionsptions.AddPolicy(policyName, permissionPolicy);

        return permissionPolicy;
    }
}