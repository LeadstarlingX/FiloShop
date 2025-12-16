using FiloShop.SharedKernel.Api;
using FiloShop.SharedKernel.CQRS.Commands;
using FiloShop.SharedKernel.CQRS.Queries;
using FiloShop.SharedKernel.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Abstractions;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected ApiController(ISender sender)
    {
        Sender = sender;
    }

    protected async Task<IActionResult> Dispatch<T>(ICommand<T> command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return FormatResponse(result);
    }
    
    protected async Task<IActionResult> Dispatch<T>(IQuery<T> query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return FormatResponse(result);
    }

    protected IActionResult FormatResponse<T>(Result<T> result)
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.Ok(result.Value, new ResponseMetadata { TraceId = traceId }));
        }

        return BadRequest(ApiResponse<T>.Fail(result.Error.Code, result.Error.Message, traceId: traceId));
    }
    
    protected IActionResult FormatResponse(Result result)
    {
        var traceId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        if (result.IsSuccess)
        {
            return Ok(ApiResponse.Ok(new ResponseMetadata { TraceId = traceId }));
        }

        return BadRequest(ApiResponse.Fail(result.Error.Code, result.Error.Message, traceId: traceId));
    }
}
