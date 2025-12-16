using NetArchTest.Rules;

namespace FiloShop.ArchitectureTests;

public class ApplicationArchitectureTests : ArchitectureTest
{
    [Test]
    public async Task Application_Should_Not_Depend_On_Infrastructure()
    {
        var result1 = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructurePersistenceNamespace).GetResult();
        
        var result2 = Types.InAssembly(ApplicationAssembly)
            .ShouldNot() 
            .HaveDependencyOn(InfrastructureServicesNamespace)
            .GetResult();

        await Assert.That(result1.IsSuccessful & result2.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task Application_Should_Not_Depend_On_Presentation()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationNamespace)
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
    
    [Test]
    public async Task Application_Should_Not_Depend_On_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn(ApiNamespace)
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
}
