using FiloShop.Domain.Shared.Errors;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record Email
{
    
    public string Value { get; }

    private Email(string value)
    {
        this.Value = value;
    }

    public static Result<Email> Create(string value)
    {
        var result = Validate(value);
        if(result.IsFailure)
            return Result.Failure<Email>(result.Error);

        return new Email(value);
    }
    
    public static Result Validate(string value)
    {
        var trimmedEmail = value.Trim();

        if (trimmedEmail.EndsWith(".")) {
            return Result.Failure(EmailErrors.NotValid); // suggested by @TK-421
        }
        try {
            var addr = new System.Net.Mail.MailAddress(value);
            if(addr.Address != trimmedEmail)
                return Result.Failure(EmailErrors.NotValid);
            
            return Result.Success();
        }
        catch {
            return Result.Failure(EmailErrors.NotValid);
        }
        
    }
}