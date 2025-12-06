using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.CatalogItems.CreateCatalogItem;

public sealed record CreateCatalogItemCommand(
    Guid CatalogBrandId,
    Guid CatalogTypeId ,
    Description Description,
    Name Name,
    Url PictureUri,
    string PictureBase64,
    Name PictureName ,
    Money Price) : ICommand<Guid>;