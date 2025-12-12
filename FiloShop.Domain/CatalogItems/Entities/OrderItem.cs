using FiloShop.Domain.Orders.Entities;
using FiloShop.Domain.Orders.ValueObjects;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.CatalogItems.Entities;

public sealed class OrderItem : BaseEntity
{

    private OrderItem()
    {
    }

    private OrderItem(Guid id, Guid orderId, CatalogItemOrdered catalogItemOrdered,
        Money unitPrice, int units) : base(id)
    {
        CatalogItemOrdered = catalogItemOrdered;
        UnitPrice = unitPrice;
        Units = units;
        OrderId = orderId;
    }

    public static Result<OrderItem> Create(Order order, CatalogItemOrdered catalogItemOrdered,
        Money unitPrice, int units)
    {
        return new OrderItem(Guid.NewGuid(), order.Id,catalogItemOrdered, unitPrice, units);
    }

    #region Properties
        
    public Guid OrderId { get; private set; }
    public CatalogItemOrdered CatalogItemOrdered { get; private set; }
    public Money UnitPrice { get; private set; }
    public int Units { get; private set; }
    
    #endregion
}