using NetArchTest.Rules;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.CQRS.Queries;

namespace FiloShop.NamingTests;

public class CQRSNamingTests : NamingTest
{
    [Test]
    public async Task Commands_Should_Have_Command_Suffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommand))
            .Or()
            .ImplementInterface(typeof(ICommand<>))
            .Should()
            .HaveNameEndingWith("Command")
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task Queries_Should_Have_Query_Suffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQuery<>))
            .Should()
            .HaveNameEndingWith("Query")
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task CommandHandlers_Should_Have_CommandHandler_Suffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task QueryHandlers_Should_Have_QueryHandler_Suffix()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();
        
        
        var persistenceResult = Types.InAssembly(InfrastructurePersistenceAssembly)
             .That()
             .ImplementInterface(typeof(IQueryHandler<,>))
             .Should()
             .HaveNameEndingWith("QueryHandler")
             .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
        await Assert.That(persistenceResult.IsSuccessful).IsTrue();
    }
}
