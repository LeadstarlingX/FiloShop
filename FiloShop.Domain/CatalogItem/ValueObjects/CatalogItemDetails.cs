using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Domain.CatalogItem.ValueObjects;

public sealed record CatalogItemDetails(
    Name? Name,
    Description? Description,
    Money Price
    );