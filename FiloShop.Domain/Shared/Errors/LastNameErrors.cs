using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class LastNameErrors
{
    public static readonly Error Empty = new(
        "LastName.Empty", "Last name cannot be empty");
    
    public static readonly Error MaximumLength = new(
        "LastName.MaximumLength", "Maximum length is 50");
}