using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.Users.Events;

public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;