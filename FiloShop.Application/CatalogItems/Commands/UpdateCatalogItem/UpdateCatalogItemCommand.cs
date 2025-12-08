using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.CatalogItems.Commands.UpdateCatalogItem;

public record UpdateCatalogItemCommand(
    Guid IdempotencyKey,
    Guid Id,
    Guid CatalogBrandId,
    Guid CatalogTypeId,
    Description Description,
    Name Name,
    string PictureBase64,
    Url PictureUri,
    Name PictureName,
    Money Price
    ) :  IIdempotentRequest;