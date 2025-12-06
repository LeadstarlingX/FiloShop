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

    private User(Guid id, bool isActive) : base(id)
    {
        IsActive = isActive;
    }

    public static Result<User> Create(bool isActive)
    {   
        var user = new User(Guid.NewGuid(), isActive);
        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return user;
    }

    #region Properties
    
    public bool IsActive { get; private set; } = true;

    #endregion
    
    
}