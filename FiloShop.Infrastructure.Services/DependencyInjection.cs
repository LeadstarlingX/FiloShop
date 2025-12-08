using FiloShop.Application.Authentication;
using FiloShop.Infrastructure.Services.Authentication;
using FiloShop.Infrastructure.Services.Authorization;
using FiloShop.Infrastructure.Services.Providers.Outbox;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using AuthenticationOptions = FiloShop.Infrastructure.Services.Authentication.AuthenticationOptions;
using AuthenticationService = FiloShop.Infrastructure.Services.Authentication.AuthenticationService;
using IAuthenticationService = FiloShop.Application.Authentication.IAuthenticationService;

namespace FiloShop.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IUserContext, UserContext>();

        services.AddMyAuthentication(configuration)
            .AddMyAuthorization()
            .AddHttpContextAccessor();
        
        return services;
    }

    private static IServiceCollection AddMyAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
            {
                var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

                httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        return services;
    }

    private static IServiceCollection AddMyAuthorization(this IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();

        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionsAuthorizationPolicyProvider>();

        return services;
    }
    
    private static IServiceCollection AddMyCaching(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cache") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
    
    
    private static IServiceCollection AddMyBackgroundJobs(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }

}