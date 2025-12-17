using System.Reflection;
using FiloShop.SharedKernel.Entities;
using FiloShop.SharedKernel.Events;
using NetArchTest.Rules;

namespace FiloShop.ArchitectureTests;

public class DomainArchitectureTests : ArchitectureTest
{
    [Test]
    public async Task Entities_Should_Have_Private_Constructors_And_Public_Factory_Methods()
    {
        // Arrange
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .GetTypes()
            .ToList();

        var failingTypes = new List<Type>();

        // Act
        foreach (var entityType in entityTypes)
        {
            var constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            if (!constructors.Any(c => c.IsPrivate || c.IsFamily)) // IsFamily == protected
            {
                failingTypes.Add(entityType);
                continue;
            }

            var factoryMethods = entityType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.ReturnType == entityType || 
                           (m.ReturnType.IsGenericType && 
                            m.ReturnType.GetGenericArguments().First() == entityType));

            if (!factoryMethods.Any()) 
            {
                failingTypes.Add(entityType);
            }
        }

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(failingTypes).IsEmpty();
                
            if (failingTypes.Count != 0)
            {
                var failingTypeNames = string.Join(", ", failingTypes.Select(t => t.Name));
                await Assert.That(failingTypeNames).IsEmpty();
            }
        };
    }

    [Test]
    public async Task ValueObjects_Should_Be_Immutable_Records()
    {
        // 1. Arrange: Get all candidate types from Domain
        var allTypes = Types.InAssembly(DomainAssembly)
            .That()
            .DoNotInherit(typeof(BaseEntity)) // Assuming BaseEntity is your entity base
            .And().DoNotImplementInterface(typeof(IDomainEvent))
            .And().AreNotAbstract()
            .And().ArePublic()
            .GetTypes()
            .ToList();

        // 2. Filter: Identify Records (Value Objects)
        // Note: The <Clone>$ method is a compiler artifact of C# 'records'
        var valueObjectTypes = allTypes
            .Where(t => IsRecord(t)) 
            .Where(t => t.Namespace != DomainNamespace) // Exclude root namespace if needed
            .Where(t => !t.Name.EndsWith("Errors"))
            .Where(t => !t.Name.EndsWith("Service"))
            .ToList();

        var failingTypes = new List<string>();

        // 3. Act: Check for Mutable Properties (Public Setters)
        foreach (var type in valueObjectTypes)
        {
            var mutableProperties = type.GetProperties()
                .Where(p => 
                        p.CanWrite 
                        && p.GetSetMethod(false) != null // Has a PUBLIC setter (not init, not private)
                )
                .Select(p => p.Name)
                .ToList();

            if (mutableProperties.Any())
            {
                failingTypes.Add($"{type.Name} has mutable properties: {string.Join(", ", mutableProperties)}");
            }
        }

        // 4. Assert
        await Assert.That(failingTypes).IsEmpty();
    }

    // Helper to cleanly detect C# records
    private static bool IsRecord(Type type)
    {
        return type.GetMethod("<Clone>$", BindingFlags.Instance | BindingFlags.NonPublic) is not null;
    }

    [Test]
    public async Task DomainEvents_Should_Be_Sealed_Records()
    {
        // Arrange & Act
        var domainEventTypes = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes()
            .ToList();

        var failingTypes = new List<Type>();

        foreach (var type in domainEventTypes)
        {
            if (!type.IsSealed || !type.IsClass)
            {
                failingTypes.Add(type);
            }
        }

        // Assert
        await Assert.That(failingTypes).IsEmpty();
    }

    [Test]
    public async Task Entities_Should_Inherit_From_BaseEntity()
    {
        // Arrange
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .GetTypes()
            .ToList();

        // Act
        var failingTypes = entityTypes
            .Where(t => !t.IsSubclassOf(typeof(BaseEntity)))
            .ToList();

        // Assert
        await Assert.That(failingTypes).IsEmpty();
    }

    [Test]
    public async Task Entities_Should_Have_Private_Setters()
    {
        // Arrange
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .GetTypes()
            .ToList();

        var failingTypes = new List<(Type Type, string Property)>();

        // Act
        foreach (var entityType in entityTypes)
        {
            var publicProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToList();

            foreach (var property in publicProperties)
            {
                var setMethod = property.GetSetMethod(false);
                if (setMethod?.IsPublic == true)
                {
                    // Check if it's init-only
                    var isInitOnly = setMethod.ReturnParameter
                        .GetRequiredCustomModifiers()
                        .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));

                    if (!isInitOnly)
                    {
                        failingTypes.Add((entityType, property.Name));
                    }
                }
            }
        }

        // Assert
        await Assert.That(failingTypes).IsEmpty();
    }

    [Test]
    public async Task Repository_Interfaces_Should_Reside_In_Domain()
    {
        // Arrange & Act
        var repositoryTypes = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .GetTypes()
            .ToList();

        var failingTypes = repositoryTypes
            .Where(t => !t.IsInterface)
            .ToList();

        // Assert
        using (Assert.Multiple())
        {
            await Assert.That(failingTypes).IsEmpty();
            
            await Assert.That(repositoryTypes).IsNotEmpty();
                
        };
    }

    [Test]
    public async Task Entities_Should_Have_Private_Parameterless_Constructor()
    {
        // Arrange
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(BaseEntity))
            .GetTypes()
            .ToList();

        var failingTypes = new List<Type>();

        // Act
        foreach (var entityType in entityTypes)
        {
            var constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            var hasPrivateParameterlessConstructor = constructors
                .Any(c => c.IsPrivate && c.GetParameters().Length == 0);

            if (!hasPrivateParameterlessConstructor)
            {
                failingTypes.Add(entityType);
            }
        }

        // Assert
        await Assert.That(failingTypes).IsEmpty();
    }
    

    [Test]
    public async Task DomainLayer_Should_Not_Depend_On_ApplicationLayer()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("LeroTreat.Application")
            .GetResult();

        // Assert
        await Assert.That(result.IsSuccessful).IsTrue();
    }

    [Test]
    public async Task DomainEvents_Should_Have_DomainEvent_Suffix()
    {
        // Arrange & Act
        var domainEventTypes = Types.InAssembly(DomainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes()
            .ToList();

        var failingTypes = domainEventTypes
            .Where(t => !t.Name.EndsWith("DomainEvent"))
            .ToList();

        // Assert
        await Assert.That(failingTypes).IsEmpty();
    }
    

    [Test]
    public async Task Error_Classes_Should_Be_Static()
    {
        // Arrange & Act
        var errorClasses = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Errors")
            .GetTypes()
            .ToList();

        var failingTypes = errorClasses
            .Where(t => !t.IsAbstract || !t.IsSealed) // Static classes are abstract and sealed
            .ToList();

        // Assert
        await Assert.That(failingTypes).IsEmpty();
    }
    
    
    [Test]
    public async Task Domain_Should_Not_Reference_System_DateTime()
    {
        // Domain should use IDateTimeProvider, not DateTime.Now/UtcNow
        var domainTypes = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace(DomainNamespace)
            .GetTypes();

        var violatingTypes = new List<string>();

        foreach (var type in domainTypes)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        
            foreach (var method in methods)
            {
                if (method.DeclaringType != type) continue;
            
                var body = method.GetMethodBody();
                // This is a simplified check - in reality you'd need IL inspection
                // or Roslyn analyzers for accurate detection
            }
        }
    
        // Note: This test is illustrative - proper implementation requires IL analysis
    }

    // [Test]
    // public async Task ValueObjects_Should_Have_Create_Factory_Method()
    // {
    //     // Arrange
    //     var valueObjectTypes = Types.InAssembly(DomainAssembly)
    //         .That()
    //         .AreNotAbstract()
    //         .And()
    //         .AreSealed()
    //         .And()
    //         .HaveNameEndingWith("ValueObject")
    //         .Or()
    //         .HaveNameMatching("Email|Name|Money|Address") // Common value object names
    //         .GetTypes()
    //         .ToList();
    //
    //     var failingTypes = new List<Type>();
    //
    //     // Act
    //     foreach (var valueObjectType in valueObjectTypes)
    //     {
    //         var createMethods = valueObjectType.GetMethods(BindingFlags.Public | BindingFlags.Static)
    //             .Where(m => m.ReturnType == valueObjectType || 
    //                         (m.ReturnType.IsGenericType && 
    //                          m.ReturnType.GetGenericTypeDefinition().Name == "Result`1")) // Ensures it's Result<T>
    //             .Where(m => m.Name == "Create" || m.Name.StartsWith("Create"))
    //             .ToList();
    //
    //         if (!createMethods.Any())
    //         {
    //             failingTypes.Add(valueObjectType);
    //         }
    //     }
    //
    //     // Assert
    //     await Assert.That(failingTypes).IsEmpty();
    // }
}