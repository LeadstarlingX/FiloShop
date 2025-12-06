using FiloShop.Domain.CatalogBrands.Events;
using FiloShop.Domain.CatalogBrands.ValueObjects;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.CatalogBrands.Entities;

public class CatalogBrand : BaseEntity, IAggregateRoot
{
    private CatalogBrand()
    {
    }

    private CatalogBrand(Guid id, Brand brand) : base(id)
    {
        Brand = brand;
    }

    public static Result<CatalogBrand> Create( Brand brand)
    {   
        var catalogBrand = new CatalogBrand(Guid.NewGuid(), brand);
        catalogBrand.RaiseDomainEvent(new CatalogBrandCreatedDomainEvent(catalogBrand.Id));
        return catalogBrand;
    }

    #region Properties
    
    public Brand Brand { get; private set; }
    
    #endregion
}