using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FiloShop.Domain.Baskets.Entities;
using FiloShop.Domain.Baskets.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Repositories;

public class BasketRepository : RepositoryBase<Basket>, IBasketRepository
{
    public BasketRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public BasketRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {
    }
}