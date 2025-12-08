using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Application.CatalogBrands.Queries.GetCatalogBrands;

public sealed record CatalogBrandResponse
{
    public Guid Id { get; init; }
    public required Name Name { get; init; }
}