using FiloShop.SharedKernel.Events;
using NetArchTest.Rules;

namespace FiloShop.NamingTests;

public class DomainEventNamingTests : NamingTest
{
    [Test]
    public async Task DomainEvents_Should_Be_Past_Tense()
    {
        var events = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes();

        var invalidNames = new List<string>();

        var pastTensePatterns = new[] { "Created", "Updated", "Deleted", "Changed", "Added", "Removed", "Confirmed", "Cancelled" };

        foreach (var evt in events)
        {
            var hasPastTense = pastTensePatterns.Any(pattern => evt.Name.Contains(pattern));
            if (!hasPastTense)
            {
                invalidNames.Add(evt.Name);
            }
        }

        await Assert.That(invalidNames).IsEmpty();
    }
}