using Ardalis.Specification;
using FiloShop.Domain.Orders.Entities;

namespace FiloShop.Application.Specifications.Orders;

public class OrderWithItemsByIdSpec : Specification<Order>
{
    public OrderWithItemsByIdSpec(Guid orderId)
    {
        Query
            .Where(order => order.Id == orderId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.CatalogItemOrdered);
    }
}