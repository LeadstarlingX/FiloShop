using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.CatalogTypes.Events;

public sealed record CatalogTypeCreatedDomainEvent(Guid CatalogTypeId) : IDomainEvent;