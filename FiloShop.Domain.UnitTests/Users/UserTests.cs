using FiloShop.Domain.UnitTests.Infrastructure;
using FiloShop.Domain.Users.Entities;
using FiloShop.Domain.Users.Events;
using FiloShop.Domain.Users.ValueObjects;
using FluentAssertions;

namespace FiloShop.Domain.UnitTests.Users;

public class UserTests
{
    [Test]
    public void Create_Should_ReturnUser_When_DataIsValid()
    {
        // Act
        var result = User.Create(
            DomainTestFactory.Users.ValidFirstName, 
            DomainTestFactory.Users.ValidLastName, 
            DomainTestFactory.Users.ValidEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FirstName.Should().Be(DomainTestFactory.Users.ValidFirstName);
    }

    [Test]
    public void Create_Should_RaiseUserCreatedDomainEvent()
    {
        // Act
        var result = User.Create(
            DomainTestFactory.Users.ValidFirstName, 
            DomainTestFactory.Users.ValidLastName, 
            DomainTestFactory.Users.ValidEmail);

        // Assert
        var domainEvent = result.Value.GetDomainEvents().FirstOrDefault(e => e is UserCreatedDomainEvent);
        domainEvent.Should().NotBeNull();
        ((UserCreatedDomainEvent)domainEvent!).UserId.Should().Be(result.Value.Id);
    }

    [Test]
    public void Create_Should_AddRegisteredRole()
    {
        // Act
        var result = User.Create(
            DomainTestFactory.Users.ValidFirstName, 
            DomainTestFactory.Users.ValidLastName, 
            DomainTestFactory.Users.ValidEmail);

        // Assert
        result.Value.Roles.Should().Contain(Role.Registered);
    }
}
