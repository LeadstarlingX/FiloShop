using Ardalis.Specification;
using FiloShop.Domain.Orders.Entities;

namespace FiloShop.Application.Specifications.Customer;

public class CustomerOrdersSpecification : Specification<Order>
{
    public CustomerOrdersSpecification(Guid userId)
    {
        Query.Where(o => o.UserId == userId)
            .Include(o => o.OrderItems);
    }
}