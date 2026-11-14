using UserManagement.Api.DataTransferObjects;

namespace UserManagement.Api.Contracts;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
