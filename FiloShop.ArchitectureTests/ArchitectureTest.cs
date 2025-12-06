using Assembly = System.Reflection.Assembly;

namespace FiloShop.ArchitectureTests;

public abstract class ArchitectureTest
{
    protected const string DomainNamespace = "FiloShop.Domain";
    protected const string ApplicationNamespace = "FiloShop.Application";
    protected const string InfrastructureServicesNamespace = "FiloShop.Infrastructure.Services";
    protected const string InfrastructurePersistenceNamespace = "FiloShop.Infrastructure.Persistence";
    protected const string SharedKernelNamespace = "FiloShop.SharedKernel";
    protected const string PresentationNamespace = "FiloShop.Presentation";
    protected const string ApiNamespace = "FiloShop.Api";

    protected static readonly Assembly DomainAssembly = typeof(FiloShop.Domain.Users.Entities.User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(FiloShop.Application.DependencyInjection).Assembly; 
    protected static readonly Assembly InfrastructureServicesAssembly = typeof(FiloShop.Infrastructure.Services.DependencyInjection).Assembly;
    protected static readonly Assembly InfrastructurePersistenceAssembly = typeof(FiloShop.Infrastructure.Persistence.DependencyInjection).Assembly;
    protected static readonly Assembly SharedKernelAssembly = typeof(FiloShop.SharedKernel.Entities.BaseEntity).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(FiloShop.Presentation.DependencyInjection).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(FiloShop.Api.DependencyInjection).Assembly;
}