using FiloShop.Domain.CatalogItem.Entities;
using FiloShop.Domain.Orders.Events;
using FiloShop.Domain.Orders.ValueObjects;
using FiloShop.Domain.Users.Entities;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Results;

namespace FiloShop.Domain.Orders.Entities;

public class Order : BaseEntity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = new List<OrderItem>();
    
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();


    private Order()
    {
    }

    private Order(Guid id, Guid userId, Address shipToAddress) : base(id)
    {
        UserId = userId;
        ShipToAddress = shipToAddress;
    }

    public static Result<Order> Create(User user, Address shipToAddress)
    {
        var order = new Order(Guid.NewGuid(), user.Id, shipToAddress);
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id));
        return order;
    }

    #region Properties

    public Guid UserId { get; private set; }
    public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;
    public Address ShipToAddress { get; private set; }

    #endregion
    
    // public Result<Money> Total()
    // {
    //     var total = Money.Create(0m, Currency.FromCode("USD").Value).Value;
    //     foreach (var item in _orderItems)
    //     {
    //         Money.Add(total, item.UnitPrice * item.Units);
    //     }
    //     return total;
    // }
}