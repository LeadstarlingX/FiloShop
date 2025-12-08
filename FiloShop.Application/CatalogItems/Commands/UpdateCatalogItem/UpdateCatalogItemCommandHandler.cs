using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.Commands.UpdateCatalogItem;

public class UpdateCatalogItemCommandHandler : ICommandHandler<UpdateCatalogItemCommand>
{
    public Task<Result> Handle(UpdateCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO: Implement
         */
        throw new NotImplementedException();
    }
}