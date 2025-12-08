using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.DeleteCatalogItem;

internal sealed class DeleteCatalogItemCommandHandler : ICommandHandler<DeleteCatalogItemCommand>
{
    public Task<Result> Handle(DeleteCatalogItemCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}