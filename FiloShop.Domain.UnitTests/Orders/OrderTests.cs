using FiloShop.Domain.Orders.Entities;
using FiloShop.Domain.Orders.Events;
using FiloShop.Domain.Orders.ValueObjects;
using FiloShop.Domain.UnitTests.Infrastructure;
using FluentAssertions;

namespace FiloShop.Domain.UnitTests.Orders;

public class OrderTests
{
    [Test]
    public void Create_Should_ReturnOrder_When_DataIsValid()
    {
        // Arrange
        var user = DomainTestFactory.Users.CreateValidUser();
        var address = new Address("Country", "Street", "City", "State", "Zip");

        // Act
        var result = Order.Create(user, address);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.ShipToAddress.Should().Be(address);
    }

    [Test]
    public void Create_Should_RaiseOrderCreatedDomainEvent()
    {
        // Arrange
        var user = DomainTestFactory.Users.CreateValidUser();
        var address = new Address("Country", "Street", "City", "State", "Zip");

        // Act
        var result = Order.Create(user, address);

        // Assert
        var domainEvent = result.Value.GetDomainEvents().FirstOrDefault(e => e is OrderCreatedDomainEvent);
        domainEvent.Should().NotBeNull();
        ((OrderCreatedDomainEvent)domainEvent!).OrderId.Should().Be(result.Value.Id);
    }
}
