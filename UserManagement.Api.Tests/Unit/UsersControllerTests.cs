using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserManagement.Api.Contracts;
using UserManagement.Api.Controllers;

namespace UserManagement.Api.Tests.Unit;

public class UsersControllerTests
{
    private readonly IUserService _service = Substitute.For<IUserService>();
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _controller = new UsersController(_service);
    }

    private static ControllerContext CreateUserContext(Guid userId, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }
}
