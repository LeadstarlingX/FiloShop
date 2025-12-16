using NetArchTest.Rules;
using Microsoft.EntityFrameworkCore;

namespace FiloShop.NamingTests;

public class GeneralNamingTests : NamingTest
{
    [Test]
    public async Task Repositories_Should_Have_Repository_Suffix()
    {
        // Check Interfaces in Domain
        var interfacesResult = Types.InAssembly(DomainAssembly)
            .That()
            .AreInterfaces()
            .And()
            .HaveNameMatching("Repository")
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();
        
        // Check Implementations in Infrastructure
        // Usually implementations implement the Domain interface.
        // Let's just check if anything ending in Repository matches.
        
        var implementationsResult = Types.InAssembly(InfrastructurePersistenceAssembly)
            .That()
            .AreClasses()
            .And()
            .ImplementInterface(typeof(FiloShop.Domain.Users.IRepository.IUserRepository)) 
            .Should()
            .HaveNameEndingWith("Repository")
            .GetResult();
        
        
        await Assert.That(interfacesResult.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task DbContexts_Should_Have_DbContext_Suffix()
    {
        var result = Types.InAssembly(InfrastructurePersistenceAssembly)
            .That()
            .Inherit(typeof(DbContext))
            .Should()
            .HaveNameEndingWith("DbContext")
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
}
