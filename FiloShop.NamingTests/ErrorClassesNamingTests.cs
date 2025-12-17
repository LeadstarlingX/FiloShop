using NetArchTest.Rules;

namespace FiloShop.NamingTests;

public class ErrorClassesNamingTests : NamingTest
{
    [Test]
    public async Task Error_Classes_Should_End_With_Errors()
    {
        var errorClasses = Types.InAssembly(DomainAssembly)
            .That()
            .AreClasses()
            .And()
            .ArePublic()
            .And()
            .ResideInNamespaceEndingWith(".Errors")
            .Should()
            .HaveNameEndingWith("Errors")
            .GetResult();

        await Assert.That(errorClasses.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task Error_Classes_Should_Be_Static()
    {
        var errorClasses = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Errors")
            .GetTypes();

        var nonStaticErrors = errorClasses
            .Where(t => !t.IsAbstract || !t.IsSealed)
            .ToList();

        await Assert.That(nonStaticErrors).IsEmpty();
    }
}