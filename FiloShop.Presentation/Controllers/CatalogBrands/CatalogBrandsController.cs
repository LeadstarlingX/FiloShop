using Asp.Versioning;
using FiloShop.Application.CatalogBrands.Queries.GetCatalogBrands;
using FiloShop.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogBrands;

// [Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public class CatalogBrandsController : ApiController
{
    public CatalogBrandsController(ISender sender) : base(sender)
    {
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCatalogBrands(CancellationToken cancellationToken)
    {
        return await Dispatch(new GetCatalogBrandsQuery(), cancellationToken);
    }
}