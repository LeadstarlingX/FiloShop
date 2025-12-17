using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.CQRS.Queries;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Interfaces;
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
    
    
    /// There's a problem with this test in CreateCatalogItemCommand,
    ///     comaperd with UpdateCatalogItemc, yet needs more work
    
    // [Test]
    // public async Task Commands_Should_Not_Return_Domain_Entities()
    // {
    //     // Commands should return Result<Guid> or Result, NOT Result<Order>
    //     var result = Types.InAssembly(ApplicationAssembly)
    //         .That()
    //         .ImplementInterface(typeof(ICommand<>))
    //         .Should()
    //         .NotHaveDependencyOn(DomainNamespace) // Can reference, but shouldn't return
    //         .GetResult();
    //     
    //     await Assert.That(result.IsSuccessful).IsTrue();
    // }

    [Test]
    public async Task Queries_Should_Return_DTOs_Not_Entities()
    {
        // Queries should return DTOs/Response objects, NOT domain entities
        var queryHandlers = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .GetTypes();

        var failingHandlers = new List<string>();

        foreach (var handler in queryHandlers)
        {
            var interfaces = handler.GetInterfaces();
            var queryHandlerInterface = interfaces.FirstOrDefault(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

            if (queryHandlerInterface != null)
            {
                var returnType = queryHandlerInterface.GetGenericArguments()[1]; // Result<T>
                var innerType = returnType.GetGenericArguments()[0]; // T

                // Check if T inherits from BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(innerType))
                {
                    failingHandlers.Add($"{handler.Name} returns {innerType.Name}");
                }
            }
        }

        await Assert.That(failingHandlers).IsEmpty();
    }
    
    
    [Test]
    public async Task Handlers_Should_Not_Depend_On_EF_Core_Directly()
    {
        // Handlers should use repositories, not DbContext
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Handler")
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        await Assert.That(result.IsSuccessful).IsTrue();
    }
    
    
    [Test]
    public async Task Specifications_Should_Be_In_Domain_Or_SharedKernel()
    {
        var specs = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ISpecification<>))
            .GetTypes();

        // Specifications that filter domain entities should be in domain
        // This is debatable, but worth enforcing consistency
    
        await Assert.That(specs).IsEmpty(); // Or move them to domain
    }
}
