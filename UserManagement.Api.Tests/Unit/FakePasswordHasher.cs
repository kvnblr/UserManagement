using UserManagement.Api.Security;

namespace UserManagement.Api.Tests.Unit;

public sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";
    public bool Verify(string password, string passwordHasher) =>
        passwordHasher == $"hashed:{password}";
}
