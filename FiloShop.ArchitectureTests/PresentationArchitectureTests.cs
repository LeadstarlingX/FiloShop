using FiloShop.Presentation.Abstractions;
using NetArchTest.Rules;

namespace FiloShop.ArchitectureTests;

public class PresentationArchitectureTests : ArchitectureTest
{
    [Test]
    public async Task Controllers_Should_Inherit_From_ApiController()
    {
        var result = Types.InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace("FiloShop.Presentation.Controllers")
            .And()
            .AreClasses()
            .And()
            .DoNotHaveName("BaseController") // Exclude if exists, though we use ApiController
            .Should()
            .Inherit(typeof(ApiController))
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
}
