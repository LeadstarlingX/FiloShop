using MediatR;
using Microsoft.Extensions.Logging;

namespace FiloShop.SharedKernel.Behaviors;

public class LoggingNotificationPublisher : INotificationPublisher
{
    private readonly ILogger<LoggingNotificationPublisher> _logger;

    public LoggingNotificationPublisher(ILogger<LoggingNotificationPublisher> logger)
    {
        _logger = logger;
    }

    public async Task Publish(
        IEnumerable<NotificationHandlerExecutor> handlerExecutors, 
        INotification notification, 
        CancellationToken cancellationToken)
    {
        var notificationName = notification.GetType().Name;

        foreach (var handler in handlerExecutors)
        {
            var handlerName = handler.HandlerInstance.GetType().Name;

            try
            {
                _logger.LogInformation(
                    "Publishing Domain Event: {NotificationName} to Handler: {HandlerName}", 
                    notificationName, 
                    handlerName);

                await handler.HandlerCallback(notification, cancellationToken);

                _logger.LogInformation(
                    "Successfully handled Domain Event: {NotificationName} by {HandlerName}", 
                    notificationName, 
                    handlerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Error handling Domain Event: {NotificationName} in Handler: {HandlerName}", 
                    notificationName, 
                    handlerName);
                
                throw; // Re-throw to ensure the transaction fails if an event handler fails
            }
        }
    }
}