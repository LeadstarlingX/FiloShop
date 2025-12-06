using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Users.Errors;

public class UserErrors
{
    public static Error NotFound = new(
        "User.Found",
        "The user with the specified identifier was not found");

    public static Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "The provided credentials were invalid");
}