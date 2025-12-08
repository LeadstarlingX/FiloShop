using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    Guid IdempotencyKey,
    string Email,
    string FirstName,
    string LastName,
    string Password
) : IIdempotentRequest<Guid>;