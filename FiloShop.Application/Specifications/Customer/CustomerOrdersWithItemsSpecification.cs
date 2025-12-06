using Ardalis.Specification;
using FiloShop.Domain.Orders.Entities;

namespace FiloShop.Application.Specifications.Customer;

public class CustomerOrdersWithItemsSpecification : Specification<Order>
{
    public CustomerOrdersWithItemsSpecification(Guid userId)
    {
        Query.Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.CatalogItemOrdered);
    }
}