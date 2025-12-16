using Asp.Versioning;
using FiloShop.Application.CatalogTypes.Queries.GetCatalogTypes;
using FiloShop.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogTypes;

// [Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CatalogTypesController : ApiController
{
    public CatalogTypesController(ISender sender) : base(sender)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetCatalogTypes(CancellationToken cancellationToken)
    {
        return await Dispatch(new GetCatalogTypesQuery(), cancellationToken);
    }
}