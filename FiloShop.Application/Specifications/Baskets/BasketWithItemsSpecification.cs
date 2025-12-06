using Ardalis.Specification;
using FiloShop.Domain.Baskets.Entities;

namespace FiloShop.Application.Specifications.Baskets;

public sealed class AssociatedUserSpecification : Specification<Basket>
{
    public AssociatedUserSpecification(Guid userId)
    {
        Query
            .Where(b => b.UserId == userId)
            .Include(b => b.Items);
    }
    
    
}
