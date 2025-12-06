using FiloShop.SharedKernel.Errors;

namespace FiloShop.Domain.Shared.Errors;

public static class MoneyErrors
{
    public static readonly Error CurrencyRequired = new(
        "Money.CurrencyRequired", "Currency is required");
    
    public static readonly Error NegativeAmount = new(
        "Money.NegativeAmount", "Amount cannot be negative");
    
    public static readonly Error CurrencyMismatch = new(
        "Money.CurrencyMismatch", "The currencies do not match");
    
}