using FiloShop.Application.Specifications.CatalogBrands;
using FiloShop.Application.Specifications.CatalogItems;
using FiloShop.Application.Specifications.CatalogTypes;
using FiloShop.Domain.CatalogBrands.IRepository;
using FiloShop.Domain.CatalogItems.Entities;
using FiloShop.Domain.CatalogItems.Errors;
using FiloShop.Domain.CatalogItems.IRepository;
using FiloShop.Domain.CatalogTypes.IRepository;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Application.CatalogItems.Commands.CreateCatalogItem;

internal sealed class CreateCatalogItemCommandHandler : ICommandHandler<CreateCatalogItemCommand, Guid> 
{
    private readonly ICatalogItemRepository _catalogItemRepository;
    private readonly ICatalogTypeRepository _catalogTypeRepository;
    private readonly ICatalogBrandRepository  _catalogBrandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCatalogItemCommandHandler(
        ICatalogItemRepository catalogItemRepository,
        ICatalogBrandRepository catalogBrandRepository,
        ICatalogTypeRepository catalogTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _catalogItemRepository = catalogItemRepository;
        _catalogBrandRepository = catalogBrandRepository;
        _catalogTypeRepository = catalogTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCatalogItemCommand request, CancellationToken cancellationToken)
    {
        /*
         * TODO: Implement
         */

        var catalogItemNameSpecification = new CatalogItemNameSpecification(request.Name);
        var existingCatalogItem = await _catalogItemRepository.FirstOrDefaultAsync(catalogItemNameSpecification, cancellationToken);
        
        if (existingCatalogItem is not null)
            return Result.Failure<Guid>(CatalogItemErrors.Duplicated);

        var catalogTypeIdSpecification = new CatalogTypeIdSpecification(request.CatalogTypeId);
        var catalogType = await _catalogTypeRepository.FirstOrDefaultAsync(catalogTypeIdSpecification, cancellationToken);

        if (catalogType is null)
            return Result.Failure<Guid>(CatalogItemErrors.CatalogTypeNotFound);

        var catalogBrandIdSpecification = new CatalogBrandIdSpecification(request.CatalogBrandId);
        var catalogBrand = await _catalogBrandRepository.FirstOrDefaultAsync(catalogBrandIdSpecification, cancellationToken);

        if (catalogBrand is null)
            return Result.Failure<Guid>(CatalogItemErrors.CatalogBrandNotFound);

        var newCatalogItem = CatalogItem.Create(
            request.Name, request.Description, request.Price, request.PictureUri,
            catalogType, catalogBrand
        );

        await _catalogItemRepository.AddAsync(newCatalogItem.Value, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return newCatalogItem.Value.Id;
    }
}