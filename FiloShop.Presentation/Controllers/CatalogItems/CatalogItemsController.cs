using Asp.Versioning;
using FiloShop.Application.CatalogItems.Commands.CreateCatalogItem;
using FiloShop.Application.CatalogItems.Commands.DeleteCatalogItem;
using FiloShop.Application.CatalogItems.Commands.UpdateCatalogItem;
using FiloShop.Application.CatalogItems.Queries.GetCatalogItemById;
using FiloShop.Application.CatalogItems.Queries.GetCatalogItemListPaged;
using FiloShop.Presentation.Controllers.CatalogItems.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogItems;


// [Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CatalogItemsController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogItemsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalogItemById(Guid catalogItemId, CancellationToken cancellationToken)
    {
        var query = new GetCatalogItemByIdQuery(catalogItemId);
        
        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }


    [HttpGet("id:guid")]
    public async Task<IActionResult> GetCatalogItemListPaged(
        int? pageSize, int? pageIndex, Guid? catalogBrandId, Guid? catalogTypeId
        , CancellationToken cancellationToken)
    {
        var query = new GetCatalogItemListPagedQuery(pageSize, pageIndex, catalogBrandId, catalogTypeId);
        
        var result = await _sender.Send(query, cancellationToken);
    
        return Ok(result.Value);
    }


    [HttpPost]
    public async Task<IActionResult> CreateCatalogItem(
        [FromBody] CreateCatalogItemRequest request, 
        [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var command = new CreateCatalogItemCommand(
            idempotencyKey,
            request.CatalogBrandId,
            request.CatalogTypeId ,
            request.Description,
            request.Name,
            request.PictureUri,
            request.PictureBase64,
            request.PictureName ,
            request.Price);
        
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCatalogItem(
        Guid catalogItemId, 
        [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCatalogItemCommand(idempotencyKey ,catalogItemId);
        
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.IsSuccess);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCatalogItem(
        [FromBody] UpdateCatalogItemRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCatalogItemCommand(
            idempotencyKey,
            request.Id,
            request.CatalogBrandId,
            request.CatalogTypeId,
            request.Description,
            request.Name,
            request.PictureBase64,
            request.PictureUri,
            request.PictureName,
            request.Price
            );
        
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.IsSuccess);
    }
    
}