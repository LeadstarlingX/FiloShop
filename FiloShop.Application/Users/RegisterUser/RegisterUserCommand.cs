using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password
) : ICommand<Guid>;