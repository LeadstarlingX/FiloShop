using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.CatalogItems.Commands.CreateCatalogItem;

public sealed record CreateCatalogItemCommand(
    Guid IdempotencyKey,
    Guid CatalogBrandId,
    Guid CatalogTypeId ,
    Description Description,
    Name Name,
    Url PictureUri,
    string PictureBase64,
    Name PictureName ,
    Money Price) : IIdempotentRequest<Guid>;