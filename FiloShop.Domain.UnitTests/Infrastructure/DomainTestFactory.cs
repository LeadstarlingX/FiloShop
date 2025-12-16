using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.Domain.Users.Entities;
using FiloShop.Domain.Users.ValueObjects;

namespace FiloShop.Domain.UnitTests.Infrastructure;

public static class DomainTestFactory
{
    public static class Users
    {
        public static FirstName ValidFirstName => FirstName.Create("John").Value;
        public static LastName ValidLastName => LastName.Create("Doe").Value;
        public static Email ValidEmail => Email.Create("john.doe@example.com").Value;
        
        public static User CreateValidUser()
        {
            return User.Create(ValidFirstName, ValidLastName, ValidEmail).Value;
        }
    }

    public static class Shared
    {
        public static Currency UsdCurrency => Currency.Usd;
        
        public static Money ValidMoney(decimal amount = 100m) 
        {
             return Money.Create(amount, UsdCurrency).Value;
        }
    }
}
