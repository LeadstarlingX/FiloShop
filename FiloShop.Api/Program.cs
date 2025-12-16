using OpenTelemetry.Trace;
using Serilog;
using Npgsql;

namespace FiloShop.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration))
            .ConfigureServices(services =>
            {
                services.AddOpenTelemetry()
                    .WithTracing(tracing =>
                    {
                        tracing
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddEntityFrameworkCoreInstrumentation()
                            .AddNpgsql()
                            .AddConsoleExporter();
                    });
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}