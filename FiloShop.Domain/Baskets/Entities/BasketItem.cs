using FiloShop.Domain.Baskets.Events;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Baskets.Entities;

public sealed class BasketItem : BaseEntity
{
    private BasketItem()
    {
    }

    private BasketItem(Guid id, Money unitPrice,
        int quantity, Guid catalogItemId, Guid basketId) : base(id)
    {
        UnitPrice = unitPrice;
        Quantity = quantity;
        CatalogItemId = catalogItemId;
        BasketId = basketId;
    }

    public static Result<BasketItem> Create(Money unitPrice, int quantity,
        Guid catalogItemId, Basket basket)
    {
        var basketItem = new BasketItem(Guid.NewGuid(), unitPrice, quantity, catalogItemId, basket.Id);
        basketItem.RaiseDomainEvent(new BasketCreatedDomainEvent(basketItem.Id));
        return basketItem;
    }


    #region Properties

    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public Guid CatalogItemId { get; private set; }
    public Guid BasketId { get; private set; }

    #endregion
    
    public void AddQuantity(int quantity)
    {
        // Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);

        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        // Guard.Against.OutOfRange(quantity, nameof(quantity), 0, int.MaxValue);

        Quantity = quantity;
    }
}