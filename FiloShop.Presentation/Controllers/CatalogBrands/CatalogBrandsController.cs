using Asp.Versioning;
using FiloShop.Application.CatalogBrands.Queries.GetCatalogBrands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogBrands;

// [Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CatalogBrandsController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogBrandsController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCatalogBrands(CancellationToken cancellationToken)
    {
        var query = new GetCatalogBrandsQuery();
        
        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }
}