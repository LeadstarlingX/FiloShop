using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Domain.CatalogItems.ValueObjects;

public sealed record CatalogItemDetails(
    Name? Name,
    Description? Description,
    Money Price
    );