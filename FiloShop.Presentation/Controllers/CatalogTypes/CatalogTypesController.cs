using Asp.Versioning;
using FiloShop.Application.CatalogTypes.Queries.GetCatalogTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogTypes;

// [Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CatalogTypesController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalogTypes(CancellationToken cancellationToken)
    {
        var query = new GetCatalogTypesQuery();

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }
}