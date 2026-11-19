using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Contracts;
using UserManagement.Api.DataTransferObjects;
using UserManagement.Api.Domain;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var currentUser = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        var currentUserId = Guid.TryParse(currentUser, out var userId) ? userId : Guid.Empty;

        if (!(User.IsInRole(Role.Admin.ToString()) || currentUserId == id))
        {
            return Forbid();
        }

        var user = await _service.GetByIdAsync(id, cancellationToken);

        return Ok(user);
    }
}
