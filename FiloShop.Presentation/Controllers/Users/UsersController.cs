using Asp.Versioning;
using FiloShop.Application.Users.Commands.LogInUser;
using FiloShop.Application.Users.Commands.RegisterUser;
using FiloShop.Application.Users.Queries.GetLoggedInUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloShop.Presentation.Controllers.Users;

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    // [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUser(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
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

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(
        [FromBody] LogInUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(
            request.Email, request.Password);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure) return Unauthorized();

        return Ok(result.Value);
    }
}