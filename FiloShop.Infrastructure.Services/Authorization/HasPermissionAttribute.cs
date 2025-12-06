using Microsoft.AspNetCore.Authorization;

namespace FiloShop.Infrastructure.Services.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(permission)
    {
    }
}