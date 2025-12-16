using FiloShop.SharedKernel.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace FiloShop.SharedKernel.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;

        try
        {
            using (LogContext.PushProperty("TraceId", System.Diagnostics.Activity.Current?.Id ?? Guid.NewGuid().ToString()))
            {
                _logger.LogInformation("Executing request {RequestName}", requestName);

                var result = await next();

                if (result.IsSuccess)
                    _logger.LogInformation("Request {RequestName} processed successfully", requestName);
                else
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        _logger.LogError("Request {RequestName} failed with {ErrorCode}: {ErrorMessage}", 
                            requestName, result.Error.Code, result.Error.Message);
                    }

                return result;
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Request {RequestName} processing failed", requestName);

            throw;
        }
    }
}