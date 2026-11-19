using NSubstitute;
using UserManagement.Api.Contracts;
using UserManagement.Api.Controllers;

namespace UserManagement.Api.Tests.Unit;

public class AccountControllerTests
{
    private readonly IUserService _service = Substitute.For<IUserService>();
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _controller = new AccountController(_service);
    }
}
