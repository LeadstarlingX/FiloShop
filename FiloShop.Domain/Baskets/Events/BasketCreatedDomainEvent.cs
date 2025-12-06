using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.Baskets.Events;

public sealed record BasketCreatedDomainEvent(Guid BasketId) : IDomainEvent;