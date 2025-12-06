using FiloShop.Domain.Shared.ValueObjects;

namespace FiloShop.Presentation.Controllers.CatalogItems.Requests;

public record UpdateCatalogItemRequest(
     Guid Id,
     Guid CatalogBrandId,
     Guid CatalogTypeId,
     Description Description,
     Name Name,
     string PictureBase64,
     Url PictureUri,
     Name PictureName,
     Money Price);