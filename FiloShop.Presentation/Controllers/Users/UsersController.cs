using Asp.Versioning;
using FiloShop.Application.Users.Commands.LogInUser;
using FiloShop.Application.Users.Commands.RegisterUser;
using FiloShop.Application.Users.Queries.GetLoggedInUser;
using FiloShop.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.Users;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class UsersController : ApiController
{
    public UsersController(ISender sender) : base(sender)
    {
    }

    [HttpGet("me")]
    // [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUser(CancellationToken cancellationToken)
    {
        return await Dispatch(new GetLoggedInUserQuery(), cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterUserRequest request,
        [FromHeader(Name = "X-Idempotency-Key")] Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            idempotencyKey,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Password);

        return await Dispatch(command, cancellationToken);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(
        [FromBody] LogInUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(
            request.Email, request.Password);

        return await Dispatch(command, cancellationToken);
    }
}