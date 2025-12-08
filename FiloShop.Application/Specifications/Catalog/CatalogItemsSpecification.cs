using Ardalis.Specification;
using FiloShop.Domain.CatalogItems.Entities;

namespace FiloShop.Application.Specifications.Catalog;

public class CatalogItemsSpecification : Specification<CatalogItem>
{
    public CatalogItemsSpecification(params Guid[] ids)
    {
        Query.Where(c => ids.Contains(c.Id));
    }
}