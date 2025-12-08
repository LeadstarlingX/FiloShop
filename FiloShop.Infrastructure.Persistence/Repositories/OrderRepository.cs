using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FiloShop.Domain.Orders.Entities;
using FiloShop.Domain.Orders.IRepository;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.Infrastructure.Persistence.Repositories;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public OrderRepository(DbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(dbContext, specificationEvaluator)
    {
    }
}