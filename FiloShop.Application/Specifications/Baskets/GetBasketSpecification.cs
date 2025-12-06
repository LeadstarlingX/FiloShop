using Ardalis.Specification;
using FiloShop.Domain.Baskets.Entities;

namespace FiloShop.Application.Specifications.Baskets;

public class GetBasketSpecification : Specification<Basket>
{
    public GetBasketSpecification(Guid basketId)
    {
        Query
            .Where(b => b.Id == basketId)
            .Include(b => b.Items);
    }
}