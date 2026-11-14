using UserManagement.Api.Contracts;
using UserManagement.Api.Data;
using UserManagement.Api.DataTransferObjects;
using UserManagement.Api.Security;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Exceptions;
using UserManagement.Api.Domain;

namespace UserManagement.Api.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public UserService(AppDbContext context, IPasswordHasher passwordHasher, ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var identifier = request.UserNameOrEmail.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(
                u => u.UserName == identifier || u.Email == identifier,
                cancellationToken);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = _tokenService.CreateAccessToken(user);

        return new TokenResponse(
            accessToken.Token,
            accessToken.ExpiredAtUtc,
            user.ToResponse());
    }

    public async Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        var userName = request.Name.Trim().ToLowerInvariant();
        var email = request.Email.Trim().ToLowerInvariant();

        var exists = await _context.Users.AnyAsync(
                u => u.UserName == userName || u.Email == email,
                cancellationToken);

        if (exists)
        {
            throw new DuplicateUserException("Username or email is already taken.");
        }

        var user = User.Create(
                 userName,
                 email,
                 _passwordHasher.Hash(request.Password),
                 Role.User);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return user.ToResponse();
    }


}
