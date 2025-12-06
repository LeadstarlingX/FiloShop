using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.Order.Events;

public sealed record OrderCreatedDomainEvent(Guid OrderId) : IDomainEvent;