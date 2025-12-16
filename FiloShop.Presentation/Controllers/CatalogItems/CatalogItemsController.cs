using Asp.Versioning;
using FiloShop.Application.CatalogItems.Commands.CreateCatalogItem;
using FiloShop.Application.CatalogItems.Commands.DeleteCatalogItem;
using FiloShop.Application.CatalogItems.Commands.UpdateCatalogItem;
using FiloShop.Application.CatalogItems.Queries.GetCatalogItemById;
using FiloShop.Application.CatalogItems.Queries.GetCatalogItemListPaged;
using FiloShop.Presentation.Abstractions;
using FiloShop.Presentation.Controllers.CatalogItems.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogItems;


// [Authorize]
[ApiVersion(ApiVersions.V1)]
public class CatalogItemsController : ApiController
{
    public CatalogItemsController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalogItemById(Guid catalogItemId, CancellationToken cancellationToken)
    {
        var query = new GetCatalogItemByIdQuery(catalogItemId);
        
        // Using the new helper ensures consistency
        return await Dispatch(query, cancellationToken);
    }


    [HttpGet("id:guid")]
    public async Task<IActionResult> GetCatalogItemListPaged(
        int? pageSize, int? pageIndex, Guid? catalogBrandId, Guid? catalogTypeId
        , CancellationToken cancellationToken)
    {
        var query = new GetCatalogItemListPagedQuery(pageSize, pageIndex, catalogBrandId, catalogTypeId);
        
        return await Dispatch(query, cancellationToken);
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
        
        return await Dispatch(command, cancellationToken);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCatalogItem(
        Guid catalogItemId, 
        [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCatalogItemCommand(idempotencyKey ,catalogItemId);
        
        var result = await Sender.Send(command, cancellationToken);

    
        
        return FormatResponse(result);
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
        
        var result = await Sender.Send(command, cancellationToken);

        return FormatResponse(result);
    }
    
}