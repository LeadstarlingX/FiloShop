using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Application.CatalogTypes.Queries.GetCatalogTypes;

public sealed record GetCatalogTypesResponse
{
    public Guid Id { get; init; }
    public required Name Name { get; init; }
}