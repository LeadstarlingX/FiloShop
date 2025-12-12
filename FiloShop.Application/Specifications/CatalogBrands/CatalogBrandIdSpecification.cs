using Ardalis.Specification;
using FiloShop.Domain.CatalogBrands.Entities;

namespace FiloShop.Application.Specifications.CatalogBrands;

public class CatalogBrandIdSpecification : Specification<CatalogBrand>
{
    public CatalogBrandIdSpecification(Guid catalogBrandId)
    {
        Query.Where(catalogBrand => catalogBrand.Id == catalogBrandId);
    }
}