using Ardalis.Specification;
using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Application.Specifications.CatalogItems;

public class CatalogItemNameSpecification : Specification<CatalogItem>
{
    public CatalogItemNameSpecification(Name catalogItemName)
    {
        Query.Where(item => catalogItemName == item.Name);
    }
}