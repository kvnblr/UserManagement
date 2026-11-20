using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserManagement.Api.Contracts;
using UserManagement.Api.Controllers;
using UserManagement.Api.DataTransferObjects;
using UserManagement.Api.Domain;

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

    [Fact]
    public async Task GetById_returns_forbid_when_user_is_not_owner_and_not_admin()
    {
        var ownerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        _controller.ControllerContext = CreateUserContext(currentUserId, Role.User.ToString());

        var result = await _controller.GetById(ownerId, CancellationToken.None);

        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task GetById_returns_ok_when_user_is_owner()
    {
        var userId = Guid.NewGuid();

        var response = new UserResponse(
                userId,
                "alice",
                "alice@example.com",
                Role.User.ToString(),
                DateTime.UtcNow,
                DateTime.UtcNow);

        _service
            .GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(response);

        _controller.ControllerContext = CreateUserContext(userId, Role.User.ToString());

        var result = await _controller.GetById(userId, CancellationToken.None);

        var ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }
}
