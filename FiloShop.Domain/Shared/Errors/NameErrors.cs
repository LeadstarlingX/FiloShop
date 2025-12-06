using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class NameErrors
{
    public static readonly Error Empty = new(
        "Name.Empty", "Name cannot be empty");
    
    public static readonly Error MaximumLength = new(
        "Name.MaximumLength", "Maximum length is 50");
}