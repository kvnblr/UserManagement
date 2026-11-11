namespace UserManagement.Api.Tests.Unit;

public sealed class FakePasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";
    public bool Verify(string password, string passwordHasher) =>
        passwordHasher == $"hashed:{password}";
}
