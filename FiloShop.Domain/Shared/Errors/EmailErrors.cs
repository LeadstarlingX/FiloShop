using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class EmailErrors
{
    public static readonly Error NotValid = new(
        "Email.NotValid", "Email is not in valid format");
}