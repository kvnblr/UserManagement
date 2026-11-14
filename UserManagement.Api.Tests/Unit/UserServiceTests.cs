using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserManagement.Api.Data;
using UserManagement.Api.DataTransferObjects;
using UserManagement.Api.Domain;
using UserManagement.Api.Exceptions;
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

    private static User CreateUser()
        => User.Create(
                userName: "alice",
                email: "alice@example.com",
                password: "Password123!",
                role: Role.User
                );

    [Fact]
    public async Task RegisterAsync_creates_user_with_hashed_password_and_user_role()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var request = new RegisterUserRequest(
            "Alice",
            "Alice@Example.com",
            "Password123!"
                );

        var result = await sut.RegisterAsync(request);

        result.UserName.Should().Be("alice");
        result.Email.Should().Be("alice@example.com");
        result.Role.Should().Be(Role.User.ToString());

        database.Users.Single().PasswordHash.Should().Be("hashed:Password123!");
    }

    [Fact]
    public async Task RegisterAsync_throws_when_username_exist()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        database.Users.Add(CreateUser());
        await database.SaveChangesAsync();

        var request = new RegisterUserRequest(
            "Alice",
            "Alice@Example.com",
            "Password123!");

        await sut
            .Invoking(s => s.RegisterAsync(request))
            .Should()
            .ThrowAsync<DuplicateUserException>();
    }
}
