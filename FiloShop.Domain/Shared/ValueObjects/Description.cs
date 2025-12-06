using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record Description
{
    public string Value { get; }
    
    private Description(string value)
    {
        this.Value = value;
    }

    public static Result<Description> Create(string value)
    {
        var result = Validate(value);

        if (result.IsFailure)
            return Result.Failure<Description>(result.Error);
        
        return new Description(value);
    }

    public static Result Validate(string value)
    {
        if(value.Length < 1)
            return Result.Failure(DescriptionErrors.Empty);
        
        if(value.Length > 200)
            return Result.Failure(DescriptionErrors.MaximumLength);
        
        return Result.Success();
    }
}