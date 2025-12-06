using FiloShop.SharedKernel.Events;

namespace FiloShop.Domain.CatalogBrands.Events;

public sealed record CatalogBrandCreatedDomainEvent(Guid CatalogBrandId) : IDomainEvent;