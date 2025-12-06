using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Shared.ValueObjects;

public sealed record ContactInfo
{
    public FirstName FirstName  { get; }
    public LastName LastName { get; }
    public Email Email { get; }
    public PhoneNumber PhoneNumber { get; }

    private ContactInfo(FirstName firstName, LastName lastName, Email email, PhoneNumber phoneNumber)
    {
        FirstName = firstName;
        Email = email;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public static Result<ContactInfo> Create(FirstName firstName, Email email, LastName lastName, PhoneNumber phoneNumber)
    {
        var result = Validate(firstName, lastName, email, phoneNumber);
        if (result.IsFailure)
            return Result.Failure<ContactInfo>(result.Error);
        
        return new ContactInfo(firstName, lastName, email, phoneNumber);
    }
    
    
    public static Result Validate(FirstName firstName, LastName lastName, Email email, PhoneNumber phoneNumber)
    {
        var firstNameResult = FirstName.Validate(firstName.Value);
        if(!firstNameResult.IsSuccess)
            return Result.Failure(firstNameResult.Error);
        
        var lastNameResult = LastName.Validate(lastName.Value);
        if(!lastNameResult.IsSuccess)
            return Result.Failure(lastNameResult.Error);
        
        var emailResult = Email.Validate(email.Value);
        if(!emailResult.IsSuccess)
            return Result.Failure(emailResult.Error);
        
        var phoneNumberResult = PhoneNumber.Validate(phoneNumber.Value);
        if(!phoneNumberResult.IsSuccess)
            return Result.Failure(phoneNumberResult.Error);
        
        return Result.Success();
    }
}