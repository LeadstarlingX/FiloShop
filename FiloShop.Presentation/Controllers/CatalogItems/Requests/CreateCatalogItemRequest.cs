using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Presentation.Controllers.CatalogItems.Requests;

public sealed record CreateCatalogItemRequest(
    Guid CatalogBrandId,
    Guid CatalogTypeId ,
    Description Description,
    Name Name,
    Url PictureUri,
    string PictureBase64,
    Name PictureName ,
    Money Price
    );