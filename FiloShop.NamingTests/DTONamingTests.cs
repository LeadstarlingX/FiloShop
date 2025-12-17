using NetArchTest.Rules;

namespace FiloShop.NamingTests;

public class DTONamingTests : NamingTest
{
    [Test]
    public async Task DTOs_Should_End_With_Request_Or_Response()
    {
        var dtos = Types.InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace("FiloShop.Presentation.Controllers")
            .And()
            .AreClasses()
            .And()
            .AreNotAbstract()
            .GetTypes()
            .Where(t => !t.Name.EndsWith("Controller"))
            .ToList();

        var invalidNames = dtos
            .Where(t => !t.Name.EndsWith("Request") && !t.Name.EndsWith("Response"))
            .ToList();

        await Assert.That(invalidNames).IsEmpty();
    }
}