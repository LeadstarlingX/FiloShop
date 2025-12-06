using Asp.Versioning;
using FiloShop.Application.CatalogItems.CreateCatalogItem;
using FiloShop.Application.CatalogItems.DeleteCatalogItem;
using FiloShop.Application.CatalogItems.GetCatalogItemById;
using FiloShop.Application.CatalogItems.UpdateCatalogItem;
using FiloShop.Domain.CatalogItem.Entities;
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


    [HttpGet]
    public async Task<IActionResult> CatalogItemListPaged(Guid catalogItemId, CancellationToken cancellationToken)
    {
        var query = new GetCatalogItemByIdQuery(catalogItemId);
        
        var result = await _sender.Send(query, cancellationToken);
    
        return Ok(result.Value);
    }


    [HttpPost]
    public async Task<IActionResult> CreateCatalogItem(
        CreateCatalogItemRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCatalogItemCommand(
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
    public async Task<IActionResult> DeleteCatalogItem(Guid catalogItemId, CancellationToken cancellationToken)
    {
        var command = new DeleteCatalogItemCommand(catalogItemId);
        
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.IsSuccess);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCatalogItem(UpdateCatalogItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCatalogItemCommand(
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