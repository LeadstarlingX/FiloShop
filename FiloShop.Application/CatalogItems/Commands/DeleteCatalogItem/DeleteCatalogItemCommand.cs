using FiloShop.SharedKernel.CQRS.Commands;

namespace FiloShop.Application.CatalogItems.Commands.DeleteCatalogItem;

public sealed record DeleteCatalogItemCommand(
    Guid IdempotencyKey,
    Guid CatalogItemId) : IIdempotentRequest;