using System.Text.Json;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.Idempotency;
using FiloShop.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FiloShop.SharedKernel.Behaviors;

public sealed class IdempotentCommandBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentRequest<TResponse>
    where TResponse : Result
{
    private readonly IIdempotencyStore _store;
    private readonly ILogger<IdempotentCommandBehavior<TRequest, TResponse>> _logger;

    public IdempotentCommandBehavior(
        IIdempotencyStore store,
        ILogger<IdempotentCommandBehavior<TRequest, TResponse>> logger)
    {
        _store = store;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var idempotencyKey = request.IdempotencyKey;
        var requestName = typeof(TRequest).Name;

        var cachedRecord = await _store.GetByKeyAsync(idempotencyKey, cancellationToken);

        if (cachedRecord is not null)
        {
            _logger.LogInformation(
                "Idempotency: Request {RequestName} with key {IdempotencyKey} already processed. Returning cached response",
                requestName, idempotencyKey);

            return JsonSerializer.Deserialize<TResponse>(cachedRecord.SerializedResponse)!;
        }

        _logger.LogInformation(
            "Idempotency: Processing new request {RequestName} with key {IdempotencyKey}",
            requestName, idempotencyKey);

        var response = await next();

        var record = IdempotencyRecord.Create(
            idempotencyKey,
            requestName,
            JsonSerializer.Serialize(response));

        await _store.SaveAsync(record, cancellationToken);

        return response;
    }
}