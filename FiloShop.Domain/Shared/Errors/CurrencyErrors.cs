using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class CurrencyErrors
{
    public static readonly Error NotFound = 
        new("Currency.NotFound", "Currency code not found");
}