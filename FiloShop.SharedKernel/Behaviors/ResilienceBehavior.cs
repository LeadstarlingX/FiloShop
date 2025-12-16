using MediatR;
using Polly;
using Polly.Retry;

namespace FiloShop.SharedKernel.Behaviors;

public class ResilienceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ResiliencePipeline _pipeline;

    public ResilienceBehavior()
    {
        // Define a simple default retry pipeline for now. 
        // In a real scenario, this might be injected via IResiliencePipelineProvider
        _pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(), // Handle all exceptions, or filter for specific ones
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true
            })
            .Build();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return await _pipeline.ExecuteAsync(async ct => await next(), cancellationToken);
    }
}
