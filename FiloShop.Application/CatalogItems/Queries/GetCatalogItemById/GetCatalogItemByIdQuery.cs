using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogItems.Queries.GetCatalogItemById;

public record GetCatalogItemByIdQuery(Guid CatalogItemId) : ICachedQuery<CatalogItemResponse>
{
    public string CacheKey => $"catalog-item-{CatalogItemId}";
    public TimeSpan? Expiration => null;
}