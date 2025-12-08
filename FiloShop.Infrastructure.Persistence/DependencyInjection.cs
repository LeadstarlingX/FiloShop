using Dapper;
using FiloShop.Domain.Baskets.IRepository;
using FiloShop.Domain.CatalogBrands.IRepository;
using FiloShop.Domain.CatalogItems.IRepository;
using FiloShop.Domain.CatalogTypes.IRepository;
using FiloShop.Domain.Orders.IRepository;
using FiloShop.Domain.Users.IRepository;
using FiloShop.Infrastructure.Persistence.AppDbContext;
using FiloShop.Infrastructure.Persistence.Idempotency;
using FiloShop.Infrastructure.Persistence.Providers.Data;
using FiloShop.Infrastructure.Persistence.Repositories;
using FiloShop.SharedKernel.Idempotency;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiloShop.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructurePersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddMediatR(configurations =>
            configurations.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        
        services.AddScoped<IIdempotencyStore, IdempotencyStore>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICatalogTypeRepository, CatalogTypeRepository>();
        services.AddScoped<ICatalogItemRepository, CatalogItemRepository>();
        services.AddScoped<ICatalogBrandRepository, CatalogBrandRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        
        services.AddDatabase(configuration);
        
        
        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(configuration.GetConnectionString("DefaultConnection")!));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

        return services;
    }
    
    
    /*
     * TODO: Check the placement of healthcheck
     */
    
    // private static IServiceCollection AddMyHealthCheck(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddHealthChecks()
    //         .AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);
    //
    //     return services;
    // }
    
}