using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.CatalogItems.UpdateCatalogItem;

public record UpdateCatalogItemCommand(
    Guid Id,
    Guid CatalogBrandId,
    Guid CatalogTypeId,
    Description Description,
    Name Name,
    string PictureBase64,
    Url PictureUri,
    Name PictureName,
    Money Price
    ) : ICommand;