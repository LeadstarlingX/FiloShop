using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Interfaces;
using FiloShop.SharedKernel.Providers;
using FiloShop.SharedKernel.Resilience;
using MediatR;
using Newtonsoft.Json;

namespace FiloShop.SharedKernel.Behaviors;

public class DlqBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse> 
{
    private readonly IDeadLetterQueueWriter _deadLetterQueueWriter;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DlqBehavior(IDeadLetterQueueWriter deadLetterQueueWriter, IDateTimeProvider dateTimeProvider)
    {
        _deadLetterQueueWriter = deadLetterQueueWriter;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            await SaveToDeadLetterQueue(request, ex, cancellationToken);
            throw;
        }
    }

    private async Task SaveToDeadLetterQueue(TRequest request, Exception exception, CancellationToken cancellationToken)
    {
        var content = JsonConvert.SerializeObject(request, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        var message = new DeadLetterMessage(
            Guid.NewGuid(),
            _dateTimeProvider.UtcNow,
            request.GetType().Name,
            content,
            exception.ToString());

        await _deadLetterQueueWriter.WriteAsync(message, cancellationToken);
    }
}
