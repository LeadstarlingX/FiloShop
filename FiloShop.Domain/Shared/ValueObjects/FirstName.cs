using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record FirstName
{
    public string Value { get; }

    private FirstName(string value)
    {
        this.Value = value;
    }

    public static Result<FirstName> Create(string value)
    {
        var result = Validate(value);
        
        if(result.IsFailure)
            return Result.Failure<FirstName>(result.Error);

        return new FirstName(value);
    }
    
    public static Result Validate(string value)
    {
        if(value.Length < 1)
            return Result.Failure(FirstNameErrors.Empty);
        
        if(value.Length > 50)
            return Result.Failure(FirstNameErrors.MaximumLength);
        
        return Result.Success();
    }
}