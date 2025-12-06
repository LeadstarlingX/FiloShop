using Ardalis.Specification;
using FiloShop.Domain.CatalogItem.Entities;

namespace FiloShop.Application.Specifications.Catalog;

public class CatalogItemNameSpecification : Specification<CatalogItem>
{
    public CatalogItemNameSpecification(string catalogItemName)
    {
        Query.Where(item => catalogItemName == item.Name.Value);
    }
}