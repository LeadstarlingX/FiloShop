using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }
    
    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public static Result<Money> Create(decimal amount, Currency currency)
    {
        var result = Validate(amount, currency);
        if(result.IsFailure)
            return Result.Failure<Money>(result.Error);
        
        return new Money(amount, currency);
    }

    public static Result Validate(decimal amount, Currency currency)
    {
        if (currency == Currency.None)
            return Result.Failure<Money>(MoneyErrors.CurrencyRequired);
        
        if (amount < 0)
            return Result.Failure<Money>(MoneyErrors.NegativeAmount);
        
        return Result.Success();
    }
    
    public static Result<Money> Add(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            return Result.Failure<Money>(MoneyErrors.CurrencyMismatch);
        
        return Create(a.Amount + b.Amount, a.Currency);
    }

    public static Money Zero()
    {
        return new Money(0, Currency.None);
    }

    public static Money Zero(Currency currency)
    {
        return new Money(0, currency);
    }

    public bool IsZero()
    {
        return this == Zero(Currency);
    }
}