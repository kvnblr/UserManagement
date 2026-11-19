using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserManagement.Api.Contracts;
using UserManagement.Api.Controllers;
using UserManagement.Api.DataTransferObjects;
using UserManagement.Api.Domain;

namespace UserManagement.Api.Tests.Unit;

public class AccountControllerTests
{
    private readonly IUserService _service = Substitute.For<IUserService>();
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _controller = new AccountController(_service);
    }

    [Fact]
    public async Task Register_returns_created_with_user()
    {
        var request = new RegisterUserRequest(
                "alice",
                "alice@example.com",
                "Password123!");

        var response = new UserResponse(
                Guid.NewGuid(),
                "alice",
                "alice@example.com",
                Role.User.ToString(),
                DateTime.UtcNow,
                DateTime.UtcNow);

        _service
            .RegisterAsync(request, Arg.Any<CancellationToken>())
            .Returns(response);

        var result = await _controller.Register(request, CancellationToken.None);

        var created = result.Result as CreatedAtActionResult;

        created.Should().NotBeNull();
        created!.Value.Should().BeEquivalentTo(response);
        created.RouteValues!["id"].Should().Be(response.Id);
    }

    [Fact]
    public async Task Login_returns_ok_with_token()
    {
        var request = new LoginRequest("alice", "Password123!");

        var user = new UserResponse(
            Guid.NewGuid(),
            "alice",
            "alice@example.com",
            Role.User.ToString(),
            DateTime.UtcNow,
            DateTime.UtcNow);

        var token = new TokenResponse(
                "token",
                DateTime.UtcNow.AddHours(1),
                user);

        _service
            .LoginAsync(request, Arg.Any<CancellationToken>())
            .Returns(token);

        var result = await _controller.Login(request, CancellationToken.None);

        var ok = result.Result as OkObjectResult;

        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(token);
    }
}
