using Ardalis.Specification;
using FiloShop.Domain.CatalogItems.Entities;

namespace FiloShop.Application.Specifications.CatalogItems;

public class CatalogItemIdSpecification : Specification<CatalogItem>
{
    public CatalogItemIdSpecification(Guid catalogItemId)
    {
        Query.Where(x => x.Id == catalogItemId);
    }
}