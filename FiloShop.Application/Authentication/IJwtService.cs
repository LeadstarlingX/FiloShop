using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.Authentication;

public interface IJwtService
{
    Task<Result<string>> GetAccessTokenAsync(string email, string password,
        CancellationToken cancellationToken = default);
}