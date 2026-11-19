using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Contracts;
using UserManagement.Api.DataTransferObjects;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/account")]
public sealed class AccountController : ControllerBase
{
    private readonly IUserService _service;

    public AccountController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _service.RegisterAsync(request, cancellationToken);

        return CreatedAtAction(nameof(UsersController.GetById),
                "Users",
                new { id = user.Id },
                user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var token = await _service.LoginAsync(request, cancellationToken);
        return Ok(token);
    }

    [Authorize]
    [HttpGet("current")]
    public async Task<ActionResult<UserResponse>> CurrentUser(CancellationToken cancellationToken)
    {
        var currentUser = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(currentUser, out Guid userId))
        {
            return Unauthorized();
        }

        var user = await _service.GetByIdAsync(userId, cancellationToken);
        return Ok(user);
    }

}
