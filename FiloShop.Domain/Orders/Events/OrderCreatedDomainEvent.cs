using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.Orders.Events;

public sealed record OrderCreatedDomainEvent(Guid OrderId) : IDomainEvent;