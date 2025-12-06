using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class DescriptionErrors
{
    public static readonly Error Empty = new(
        "Description.Empty", "Description cannot be empty");
    
    public static readonly Error MaximumLength = new(
        "Description.MaximumLength", "Maximum length is 200");
}