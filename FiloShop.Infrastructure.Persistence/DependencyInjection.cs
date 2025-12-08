using Dapper;
using FiloShop.Infrastructure.Persistence.AppDbContext;
using FiloShop.Infrastructure.Persistence.Providers.Data;
using FiloShop.SharedKernel.Interfaces;
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
        
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

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