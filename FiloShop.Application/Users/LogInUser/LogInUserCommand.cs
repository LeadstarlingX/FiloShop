using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.Users.LogInUser;

public sealed record LogInUserCommand(string Email, string Password)
    : ICommand<AccessTokenResponse>;