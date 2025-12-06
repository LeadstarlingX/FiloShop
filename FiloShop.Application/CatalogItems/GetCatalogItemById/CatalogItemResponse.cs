using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Application.CatalogItems.GetCatalogItemById;

public sealed record CatalogItemResponse
{
    public int Id { get; set; }
    public Name Name { get; set; } = null!;
    public Description Description { get; set; } = null!;
    public Money Price { get; set; } = null!;
    public Url PictureUri { get; set; } = null!;
    public Guid CatalogTypeId { get; set; }
    public Guid CatalogBrandId { get; set; }
}