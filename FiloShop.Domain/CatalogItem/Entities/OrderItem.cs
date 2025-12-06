using FiloShop.Domain.Orders.ValueObjects;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.CatalogItem.Entities;

public sealed class OrderItem : BaseEntity
{

    private OrderItem()
    {
    }

    private OrderItem(Guid id, CatalogItemOrdered catalogItemOrdered,
        Money unitPrice, int units) : base(id)
    {
        CatalogItemOrdered = catalogItemOrdered;
        UnitPrice = unitPrice;
        Units = units;
    }

    public static Result<OrderItem> Create(CatalogItemOrdered catalogItemOrdered,
        Money unitPrice, int units)
    {
        return new OrderItem(Guid.NewGuid(), catalogItemOrdered, unitPrice, units);
    }

    #region Properties
    
    public CatalogItemOrdered CatalogItemOrdered { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Units { get; private set; }
    
    #endregion
}