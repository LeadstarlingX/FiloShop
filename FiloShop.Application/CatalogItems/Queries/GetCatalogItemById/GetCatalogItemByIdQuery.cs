using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogItems.Queries.GetCatalogItemById;

public class GetCatalogItemByIdQuery(Guid CatalogItemId) : IQuery<CatalogItemResponse>
{
    
}