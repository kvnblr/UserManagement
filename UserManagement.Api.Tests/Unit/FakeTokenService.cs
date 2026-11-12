using UserManagement.Api.Domain;
using UserManagement.Api.Security;

namespace UserManagement.Api.Tests.Unit;

public sealed class FakeTokenService : ITokenService
{
    public AccessToken CreateAccessToken(User user) =>
        new($"token-{user.Id:N}", DateTime.UtcNow.AddHours(1));
}
