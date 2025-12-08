using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.CatalogItems.Events;

public sealed record CatalogItemCreatedDomainEvent(Guid CatalogItemId) : IDomainEvent;