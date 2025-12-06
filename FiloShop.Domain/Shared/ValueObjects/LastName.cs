using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record LastName
{
    public string Value { get; }

    private LastName(string value)
    {
        this.Value = value;
    }

    public static Result<LastName> Create(string value)
    {
        var result = Validate(value);
        
        if(result.IsFailure)
            return Result.Failure<LastName>(result.Error);

        return new LastName(value);
    }
    
    public static Result Validate(string value)
    {
        if(value.Length < 1)
            return Result.Failure(LastNameErrors.Empty);
        
        if(value.Length > 50)
            return Result.Failure(LastNameErrors.MaximumLength);
        
        return Result.Success();
    }
}