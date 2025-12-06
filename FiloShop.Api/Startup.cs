using Asp.Versioning.ApiExplorer;
using FiloShop.Api.Extensions;
using FiloShop.Api.Middleware;
using FiloShop.Application;
using FiloShop.Infrastructure.Persistence;
using FiloShop.Infrastructure.Services;
using FiloShop.Presentation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

namespace FiloShop.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var conn = Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"\n******** Using connection string: {conn} ********\n");

        services.AddApi(Configuration)
            .AddPresentation()
            .AddInfrastructurePersistence(Configuration)
            .AddInfrastructureServices(Configuration)
            .AddApplication();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        
        
        app.UseSwaggerUI(options =>
        {
            foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
            {
                string url = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });
        
        // app.SeedData();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
        }
        
        app.ApplyMigrations();

        app.UseSerilogRequestLogging();

        app.UseMiddleware<RequestContextLoggingMiddleware>();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }
}