using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogBrands.Queries.GetCatalogBrands;

public record GetCatalogBrandsQuery : ICachedQuery<IReadOnlyList<CatalogBrandResponse>>
{
    public string CacheKey => $"brands";
    public TimeSpan? Expiration  => null;
}