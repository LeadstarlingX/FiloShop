using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }
    
    private PhoneNumber(string value)
    {
        this.Value = value;
    }
    
    public static Result<PhoneNumber> Create(string value)
    {
        var result = Validate(value);
        if (result.IsFailure)
            return Result.Failure<PhoneNumber>(result.Error);
        
        return Result.Success(new PhoneNumber(NormalizePhoneNumber(value)));
    }

    public static Result Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<PhoneNumber>(PhoneNumberErrors.Empty);
        
        var normalized = NormalizePhoneNumber(value);
        
        if (!normalized.All(char.IsDigit))
            return Result.Failure<PhoneNumber>(PhoneNumberErrors.InvalidFormat);
        
        if (normalized.Length < 10 || normalized.Length > 15)
            return Result.Failure<PhoneNumber>(PhoneNumberErrors.InvalidLength);
        
        return Result.Success();
    }
    
    private static string NormalizePhoneNumber(string value)
    {
        return new string(value.Where(c => char.IsDigit(c)).ToArray());
    }
    
    public string FormatForDisplay()
    {
        // Example: "1234567890" -> "(123) 456-7890"
        if (Value.Length == 10)
            return $"({Value[..3]}) {Value.Substring(3, 3)}-{Value.Substring(6)}";
        
        return Value;
    }
    
    private IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
    
    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}