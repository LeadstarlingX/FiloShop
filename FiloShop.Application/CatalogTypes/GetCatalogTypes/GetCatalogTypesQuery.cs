using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogTypes.GetCatalogTypes;

public record GetCatalogTypesQuery : IQuery<IReadOnlyList<GetCatalogTypesResponse>>
{
    
}