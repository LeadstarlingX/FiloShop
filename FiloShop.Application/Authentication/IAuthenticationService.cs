using FiloShop.Domain.Users.Entities;

namespace FiloShop.Application.Authentication;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(
        User user, string password, CancellationToken cancellationToken = default);
}