using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.CatalogItems;


[Authorize]
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
}