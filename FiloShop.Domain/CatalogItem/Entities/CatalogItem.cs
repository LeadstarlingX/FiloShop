using FiloShop.Domain.CatalogBrands.Entities;
using FiloShop.Domain.CatalogItem.Events;
using FiloShop.Domain.CatalogTypes.Entites;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.CatalogItem.Entities;

public sealed class CatalogItem : BaseEntity, IAggregateRoot
{

    private CatalogItem()
    {
    }

    private CatalogItem(Guid id, Name name, Description description,
        Money price, Url pictureUri, Guid catalogTypeId, Guid catalogBrandId) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        PictureUri = pictureUri;
        CatalogTypeId = catalogTypeId;
        CatalogBrandId = catalogBrandId;
    }

    public static Result<CatalogItem> Create(Name name, Description description,
        Money price, Url pictureUri, CatalogType catalogType, CatalogBrand catalogBrand)
    {
        var catalogItem = new CatalogItem(Guid.NewGuid(), name, description,
            price, pictureUri, catalogType.Id, catalogBrand.Id);
        catalogItem.RaiseDomainEvent(new CatalogItemCreatedDomainEvent(catalogItem.Id));
        return catalogItem;
    }

    #region Properties

    public Name Name { get; private set; }
    public Description Description { get; private set; }
    public Money Price { get; private set; }
    public Url PictureUri { get; private set; }
    public Guid CatalogTypeId { get; private set; }
    public Guid CatalogBrandId { get; private set; }
    // public CatalogType? CatalogType { get; private set; }
    // public CatalogBrand? CatalogBrand { get; private set; }

    #endregion
}