using Microsoft.AspNetCore.Mvc;
using UserManagement.Api.Contracts;

namespace UserManagement.Api.Controllers;

[ApiController]
public sealed class AccountController : ControllerBase
{
    private readonly IUserService _service;

    public AccountController(IUserService service)
    {
        _service = service;
    }
}
