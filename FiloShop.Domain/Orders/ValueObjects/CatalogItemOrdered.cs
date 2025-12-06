using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Orders.ValueObjects;

public sealed record CatalogItemOrdered
{
    private CatalogItemOrdered()
    {
    }


    private CatalogItemOrdered(Guid catalogItemId, Name productName, Url pictureUrl)
    {
        CatalogItemId = catalogItemId;
        ProductName = productName;
        PictureUrl = pictureUrl;
    }

    public static Result<CatalogItemOrdered> Create(CatalogItem.Entities.CatalogItem catalogItem, Name productName, Url pictureUrl)
    {
        var catalogItemOrdered = new CatalogItemOrdered(catalogItem.Id, productName, pictureUrl);
        return catalogItemOrdered;
    }

    #region Properties

    public Guid CatalogItemId { get; private set; }
    public Name ProductName { get; private set; }
    public Url PictureUrl { get; private set; }

    #endregion
    
}