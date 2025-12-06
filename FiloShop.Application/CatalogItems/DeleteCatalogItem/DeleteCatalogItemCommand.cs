using ICommand = FiloShop.SharedKernel.CQRS.Commands.ICommand;

namespace FiloShop.Application.CatalogItems.DeleteCatalogItem;

public sealed record DeleteCatalogItemCommand(Guid CatalogItemId) : ICommand;