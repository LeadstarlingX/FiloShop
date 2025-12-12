using FiloShop.Application.Specifications.CatalogItems;
using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.CatalogItems.Errors;
using FiloShop.Domain.CatalogItems.IRepository;
using FiloShop.Domain.CatalogItems.ValueObjects;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.Commands.UpdateCatalogItem;

public class UpdateCatalogItemCommandHandler : ICommandHandler<UpdateCatalogItemCommand>
{
    private readonly ICatalogItemRepository  _catalogItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCatalogItemCommandHandler(ICatalogItemRepository catalogItemRepository, IUnitOfWork unitOfWork)
    {
        _catalogItemRepository = catalogItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO: Implement
         */

        var catalogItemIdSpecification = new CatalogItemIdSpecification(request.Id);
        var existingCatalogItem = await _catalogItemRepository.FirstOrDefaultAsync(
            catalogItemIdSpecification, cancellationToken);

        if (existingCatalogItem is null)
            return Result.Failure(CatalogItemErrors.NotFound);

        var details = new CatalogItemDetails(request.Name, request.Description, request.Price); 
        existingCatalogItem.UpdateName(request.Name);
        existingCatalogItem.UpdateDescription(request.Description);
        existingCatalogItem.UpdatePrice(request.Price);
        await _catalogItemRepository.UpdateAsync(existingCatalogItem, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
        
    }
}