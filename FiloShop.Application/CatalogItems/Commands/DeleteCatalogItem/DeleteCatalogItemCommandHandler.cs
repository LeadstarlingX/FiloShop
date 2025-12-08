using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.Commands.DeleteCatalogItem;

internal sealed class DeleteCatalogItemCommandHandler : ICommandHandler<DeleteCatalogItemCommand>
{
    public Task<Result> Handle(DeleteCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO: Implement
         */
        throw new NotImplementedException();
    }
}