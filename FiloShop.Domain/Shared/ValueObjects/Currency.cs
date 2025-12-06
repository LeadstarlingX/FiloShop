using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record Currency
{
    public static readonly Currency None = new ("");
    public static readonly Currency Usd = new ("USD");
    public static readonly Currency Eur = new ("EUR");
    public static readonly Currency Syp = new ("SYP");

    
    private static readonly IReadOnlyCollection<Currency> _currencies = new []
    {
        Usd,
        Eur,
        Syp
    };
    
    private Currency(string code)
    {
        Code = code;
    }
    
    public string Code { get; init; }

    public static Result<Currency> FromCode(string code)
    {
        return _currencies.FirstOrDefault(x => x.Code == code) ??
               Result.Failure<Currency>(CurrencyErrors.NotFound);
    }
}