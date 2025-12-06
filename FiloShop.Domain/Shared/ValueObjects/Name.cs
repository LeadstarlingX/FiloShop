using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record Name
{
    public string Value { get; }
    
    private Name(string value)
    {
        this.Value = value;
    }
    
    public static Result<Name> Create(string value)
    {
        var result = Validate(value);
        
        if(result.IsFailure)
            return Result.Failure<Name>(result.Error);

        return new Name(value);
    }
    
    public static Result Validate(string value)
    {
        if(value.Length < 1)
            return Result.Failure(NameErrors.Empty);
        
        if(value.Length > 50)
            return Result.Failure(NameErrors.MaximumLength);
        
        return Result.Success();
    }
}