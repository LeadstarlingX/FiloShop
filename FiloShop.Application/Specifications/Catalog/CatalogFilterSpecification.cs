using Ardalis.Specification;
using FiloShop.Domain.CatalogItems.Entities;

namespace FiloShop.Application.Specifications.Catalog;

public class CatalogFilterSpecification : Specification<CatalogItem>
{
    public CatalogFilterSpecification(Guid? brandId, Guid? typeId)
    {
        Query.Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
                         (!typeId.HasValue || i.CatalogTypeId == typeId));
    }
}