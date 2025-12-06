using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogBrands.GetCatalogBrands;

public record GetCatalogBrandsQuery : IQuery<IReadOnlyList<CatalogBrandResponse>>
{
}