using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogTypes;

[Authorize]
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
}