using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FiloShop.Domain.CatalogTypes.Entites;
using FiloShop.Domain.CatalogTypes.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Repositories;

public class CatalogTypeRepository : RepositoryBase<CatalogType> , ICatalogTypeRepository
{
    public CatalogTypeRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public CatalogTypeRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {
    }
}