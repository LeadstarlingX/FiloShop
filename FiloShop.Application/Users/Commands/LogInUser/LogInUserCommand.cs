using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.Users.Commands.LogInUser;

public sealed record LogInUserCommand(
    Guid IdempotencyKey,
    string Email, string Password)
    : IIdempotentRequest<AccessTokenResponse>;