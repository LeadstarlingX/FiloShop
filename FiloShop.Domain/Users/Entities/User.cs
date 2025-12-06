using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.Domain.Users.Events;
using FiloShop.Domain.Users.ValueObjects;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Users.Entities;

public sealed class User : BaseEntity, IAggregateRoot
{
    private readonly List<Role> _roles = new();

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

    public string IdentityId { get; private set; } = string.Empty;
    
    private List<PaymentMethod> _paymentMethods = new List<PaymentMethod>();

    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
    
    private User()
    {
    }

    private User(Guid id ,FirstName firstName, LastName lastName,
        Email email, bool isActive) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IsActive = isActive;
    }

    public static Result<User> Create(FirstName firstName, LastName lastName, Email email)
    {   
        var user = new User(Guid.NewGuid(), firstName, lastName, email, true);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        user._roles.Add(Role.Registered);
        return user;
    }

    #region Properties
    
    public FirstName FirstName { get; private set; }
    public LastName LastName { get; private set; }
    public Email Email { get; private set; }
    public bool IsActive { get; private set; } = true;

    #endregion
    
    
}