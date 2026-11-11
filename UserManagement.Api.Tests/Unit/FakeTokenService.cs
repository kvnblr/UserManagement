namespace UserManagement.Api.Tests.Unit;

public sealed class FakeTokenService
{
    public AccessToken CreateAccessToken(User user) =>
        new($"token-{user.Id:N}", DateTime.UtcNow.AddHours(1));
}
