using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class FirstNameErrors
{
    public static readonly Error Empty = new(
        "FirstName.Empty", "First name cannot be empty");
    
    public static readonly Error MaximumLength = new(
        "FirstName.MaximumLength", "Maximum length is 50");
}