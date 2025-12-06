using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.CatalogItem.Events;

public sealed record CatalogItemCreatedDomainEvent(Guid CatalogItemId) : IDomainEvent;