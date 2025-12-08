using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.CatalogItems.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Repositories;

public class CatalogItemRepository : RepositoryBase<CatalogItem>, ICatalogItemRepository
{
    public CatalogItemRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public CatalogItemRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {
    }
}