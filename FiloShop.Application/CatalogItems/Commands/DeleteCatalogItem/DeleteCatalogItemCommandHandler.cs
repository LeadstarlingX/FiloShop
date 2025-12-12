using FiloShop.Application.Specifications.CatalogItems;
using FiloShop.Domain.CatalogItems.Errors;
using FiloShop.Domain.CatalogItems.IRepository;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;
using Microsoft.VisualBasic;

namespace FiloShop.Application.CatalogItems.Commands.DeleteCatalogItem;

internal sealed class DeleteCatalogItemCommandHandler : ICommandHandler<DeleteCatalogItemCommand>
{
    private readonly ICatalogItemRepository  _catalogItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCatalogItemCommandHandler(ICatalogItemRepository catalogItemRepository, IUnitOfWork unitOfWork)
    {
        _catalogItemRepository = catalogItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO: Implement
         */


        var catalogItemIdSpecification = new CatalogItemIdSpecification(request.CatalogItemId);
        var catalogItemToDelete = await _catalogItemRepository.FirstOrDefaultAsync(catalogItemIdSpecification, cancellationToken);

        if (catalogItemToDelete is null)
            return Result.Failure(CatalogItemErrors.NotFound);
        
        await _catalogItemRepository.DeleteAsync(catalogItemToDelete, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}