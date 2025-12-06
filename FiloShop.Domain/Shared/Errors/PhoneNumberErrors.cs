using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class PhoneNumberErrors
{
    public static readonly Error Empty = new(
        "PhoneNumber.Empty",
        "Phone number cannot be empty");
    
    public static readonly Error InvalidLength = new(
        "PhoneNumber.InvalidLength",
        "Phone number must be between 10 and 15 digits");
    
    public static readonly Error InvalidFormat = new(
        "PhoneNumber.InvalidFormat",
        "Phone number must contain only digits");
}