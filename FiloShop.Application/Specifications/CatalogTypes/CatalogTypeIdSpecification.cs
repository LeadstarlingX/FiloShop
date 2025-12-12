using Ardalis.Specification;
using FiloShop.Domain.CatalogTypes.Entites;

namespace FiloShop.Application.Specifications.CatalogTypes;

public class CatalogTypeIdSpecification : Specification<CatalogType>
{
    public CatalogTypeIdSpecification(Guid catalogTypeId)
    {
        Query.Where(x => x.Id == catalogTypeId);
    }
}