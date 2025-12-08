using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FiloShop.Domain.CatalogBrands.Entities;
using FiloShop.Domain.CatalogBrands.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Repositories;

public class CatalogBrandRepository : RepositoryBase<CatalogBrand>, ICatalogBrandRepository
{
    public CatalogBrandRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public CatalogBrandRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {
    }
}