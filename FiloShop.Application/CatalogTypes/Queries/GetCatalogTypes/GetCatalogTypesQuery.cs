using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.Application.CatalogTypes.Queries.GetCatalogTypes;

public record GetCatalogTypesQuery : IQuery<IReadOnlyList<GetCatalogTypesResponse>>
{
    
}