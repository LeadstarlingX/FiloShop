using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.UpdateCatalogItem;

public class UpdateCatalogItemCommandHandler : ICommandHandler<UpdateCatalogItemCommand>
{
    public Task<Result> Handle(UpdateCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO
         */
        
        throw new NotImplementedException();
    }
}