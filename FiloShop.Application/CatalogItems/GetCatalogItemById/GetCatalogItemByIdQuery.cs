using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogItems.GetCatalogItemById;

public class GetCatalogItemByIdQuery(Guid CatalogItemId) : IQuery<CatalogItemResponse>
{
    
}