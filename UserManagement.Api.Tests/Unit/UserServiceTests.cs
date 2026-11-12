using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.Services;
using UserManagement.Api.Tests.Unit;

public class UserServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static UserService CreateSut(AppDbContext db) =>
        new(db, new FakePasswordHasher(), new FakeTokenService());

}
