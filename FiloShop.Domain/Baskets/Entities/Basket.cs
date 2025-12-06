using FiloShop.Domain.Baskets.Events;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.Domain.Users.Entities;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Baskets.Entities;

public sealed class Basket : BaseEntity, IAggregateRoot
{
    
    private readonly List<BasketItem> _items = new List<BasketItem>();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();
    
    public int TotalItems => _items.Sum(i => i.Quantity);

    private Basket()
    {
    }

    private Basket(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
    }

    public static Result<Basket> Create(User user)
    {
        var basket = new Basket(Guid.NewGuid(), user.Id);
        basket.RaiseDomainEvent(new BasketCreatedDomainEvent(basket.Id));
        return basket;
    }

    #region Properties

    public Guid UserId { get; private set; }
    
    #endregion
    
    public void AddItem(Guid catalogItemId, Money unitPrice, int quantity = 1)
    {
        if (!Items.Any(i => i.CatalogItemId == catalogItemId))
        {
            _items.Add(BasketItem.Create(unitPrice, quantity, catalogItemId, this).Value);
            return;
        }
        var existingItem = Items.First(i => i.CatalogItemId == catalogItemId);
        existingItem.AddQuantity(quantity);
    }

    public void RemoveEmptyItems()
    {
        _items.RemoveAll(i => i.Quantity == 0);
    }

    // public void SetNewBuyerId(string buyerId)
    // {
    //     BuyerId = buyerId;
    // }
    
}