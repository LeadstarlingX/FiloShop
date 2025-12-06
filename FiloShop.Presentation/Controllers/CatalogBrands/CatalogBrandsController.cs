using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogBrands;

[Authorize]
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
}