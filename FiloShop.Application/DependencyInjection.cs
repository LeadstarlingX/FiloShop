using FiloShop.SharedKernel.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FiloShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            
            // Pipeline Behaviors (Only for Commands/Queries)
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
            
            // Notification Publisher (For Domain Events)
            configuration.NotificationPublisherType = typeof(LoggingNotificationPublisher); 
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


        return services;
    }
}