using MediatR;
using NetArchTest.Rules;

namespace FiloShop.NamingTests;

public class BehaviorNamingTests : NamingTest
{
    [Test]
    public async Task Behaviors_Should_Have_Behavior_Suffix()
    {
        // Behaviors can be in Application (e.g. Logging, Validation) or SharedKernel (e.g. Resilience, Dlq)
        
        var applicationResult = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IPipelineBehavior<,>))
            .Should()
            .HaveNameMatching("Behavior(`\\d+)?$") // Match "Behavior" or "Behavior`2"
            .GetResult();

        var sharedKernelResult = Types.InAssembly(SharedKernelAssembly)
            .That()
            .ImplementInterface(typeof(IPipelineBehavior<,>))
            .Should()
            .HaveNameMatching("Behavior(`\\d+)?$")
            .GetResult();
        
        await Assert.That(applicationResult.IsSuccessful && sharedKernelResult.IsSuccessful).IsTrue();
    }
}
