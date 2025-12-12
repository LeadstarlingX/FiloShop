using Ardalis.Specification;
using FiloShop.Domain.CatalogItems.Entities;

namespace FiloShop.Application.Specifications.CatalogItems;

public class CatalogFilterPaginatedSpecification : Specification<CatalogItem>
{
    public CatalogFilterPaginatedSpecification(int skip, int take, Guid? brandId, Guid? typeId)
        : base()
    {
        if (take == 0)
        {
            take = int.MaxValue;
        }
        Query
            .Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
                        (!typeId.HasValue || i.CatalogTypeId == typeId))
            .Skip(skip).Take(take);
    }
}