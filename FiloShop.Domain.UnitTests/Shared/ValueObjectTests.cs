using FiloShop.Domain.Shared.Errors;
using FiloShop.Domain.Shared.ValueObjects;
using FiloShop.Domain.UnitTests.Infrastructure;
using FiloShop.SharedKernel.Results;
using FluentAssertions;

namespace FiloShop.Domain.UnitTests.Shared;

public class ValueObjectTests
{
    [Test]
    public void Email_Create_Should_ReturnFailure_When_EmailIsInvalid()
    {
        var invalidEmails = new[] { "invalid-email", "test@", "@domain.com", "test.com", "user@domain." };

        foreach (var email in invalidEmails)
        {
            var result = Email.Create(email);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(EmailErrors.NotValid);
        }
    }

    [Test]
    public void Email_Create_Should_ReturnSuccess_When_EmailIsValid()
    {
        var validEmails = new[] { "user@example.com", "first.last@domain.co", "user+tag@domain.com" };

        foreach (var email in validEmails)
        {
            var result = Email.Create(email);
            result.IsSuccess.Should().BeTrue();
            result.Value.Value.Should().Be(email);
        }
    }

    [Test]
    public void Money_Add_Should_ReturnSum_When_CurrenciesMatch()
    {
        var m1 = Money.Create(100, Currency.Usd).Value;
        var m2 = Money.Create(50, Currency.Usd).Value;

        var result = Money.Add(m1, m2);

        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(150);
        result.Value.Currency.Should().Be(Currency.Usd);
    }

    [Test]
    public void Money_Add_Should_ReturnFailure_When_CurrenciesMismatch()
    {
        var m1 = Money.Create(100, Currency.Usd).Value;
        var m2 = Money.Create(50, Currency.Eur).Value;

        var result = Money.Add(m1, m2);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.CurrencyMismatch);
    }

    [Test]
    public void Money_Create_Should_ReturnFailure_When_AmountIsNegative()
    {
        var result = Money.Create(-10, Currency.Usd);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MoneyErrors.NegativeAmount);
    }

    [Test]
    public void Money_IsZero_Should_ReturnTrue_When_AmountIsZero()
    {
        var money = Money.Zero(Currency.Usd);
        money.IsZero().Should().BeTrue();
    }
    
    
}
