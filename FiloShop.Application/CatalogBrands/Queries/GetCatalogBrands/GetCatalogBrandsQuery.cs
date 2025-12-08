using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogBrands.Queries.GetCatalogBrands;

public record GetCatalogBrandsQuery : IQuery<IReadOnlyList<CatalogBrandResponse>>
{
}