using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.CreateCatalogItem;

internal sealed class CreateCatalogItemCommandHandler : ICommandHandler<CreateCatalogItemCommand, Guid> 
{
    public Task<Result<Guid>> Handle(CreateCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO
         */
        throw new NotImplementedException();
    }
}