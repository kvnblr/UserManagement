using UserManagement.Api.DataTransferObjects;

namespace UserManagement.Api.Contracts;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateUsernameAsync(Guid id, UpdateUsernameRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
