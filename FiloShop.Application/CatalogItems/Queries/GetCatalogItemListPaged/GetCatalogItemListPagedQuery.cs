using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogItems.Queries.GetCatalogItemListPaged;

public class GetCatalogItemListPagedQuery(
    int? pageSize, int? pageIndex, Guid? catalogBrandId, Guid? catalogTypeId
    ) : ICachedQuery<GetCatalogItemListPagedQuery>
{
    public string CacheKey => $"catalog-item-list-paged-{catalogBrandId}-{catalogTypeId}";
    public TimeSpan? Expiration => null;
}