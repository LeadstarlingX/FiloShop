using FiloShop.Domain.CatalogTypes.Events;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;
using Type = FiloShop.Domain.CatalogTypes.ValueObjects.Type;

namespace FiloShop.Domain.CatalogTypes.Entites;

public class CatalogType : BaseEntity, IAggregateRoot
{
    private CatalogType()
    {
    }

    private CatalogType(Guid id, Type type) : base(id)
    {
        Type = type;
    }

    public static Result<CatalogType> Create(Type type)
    {
        var catalogType = new CatalogType(Guid.NewGuid(), type);
        catalogType.RaiseDomainEvent(new CatalogTypeCreatedDomainEvent(catalogType.Id));
        return catalogType;
    }

    #region Properties

    public Type Type { get; private set; }

    #endregion
}