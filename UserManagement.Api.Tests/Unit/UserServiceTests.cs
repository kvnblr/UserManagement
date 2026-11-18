using System.Security.Authentication;
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
                password: "hashed:Password123!",
                role: Role.User
                );

    private static User CreateUser(string userName, string email) =>
        User.Create(
                userName: userName,
                email: email,
                password: "hashed:Password!23!",
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

    [Fact]
    public async Task RegisterAsync_throws_when_email_exists()
    {
        using var db = CreateContext();
        var sut = CreateSut(db);

        db.Users.Add(CreateUser());
        await db.SaveChangesAsync();

        var request = new RegisterUserRequest(
            "newuser",
            "ALICE@example.com",
            "Password123!");

        await sut
            .Invoking(s => s.RegisterAsync(request))
            .Should()
            .ThrowAsync<DuplicateUserException>();
    }

    [Fact]
    public async Task LoginAsync_returns_token_when_creditials_are_valid()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var user = CreateUser();
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var request = new LoginRequest("alice", "Password123!");

        var result = await sut.LoginAsync(request);

        result.AccessToken.Should().Be($"token-{user.Id:N}");
        result.User.Id.Should().Be(user.Id);
        result.User.UserName.Should().Be("alice");
    }

    [Fact]
    public async Task LoginAsync_throws_when_password_is_invalid()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        database.Users.Add(CreateUser());
        await database.SaveChangesAsync();

        var request = new LoginRequest("alice", "wrong-password");

        await sut
            .Invoking(s => s.LoginAsync(request))
            .Should()
            .ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task LoginAsync_throws_when_user_does_not_exists()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var request = new LoginRequest("missing", "Password123!");

        await sut
            .Invoking(s => s.LoginAsync(request))
            .Should()
            .ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task GetByIdAsync_returns_user_when_user_exists()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var user = CreateUser();
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var result = await sut.GetByIdAsync(user.Id);

        result.Id.Should().Be(user.Id);
        result.UserName.Should().Be("alice");
    }

    [Fact]
    public async Task GetByIdAsync_throws_when_user_does_not_exist()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var missingId = Guid.NewGuid();

        await sut
            .Invoking(s => s.GetByIdAsync(missingId))
            .Should()
            .ThrowAsync<UserNotFoundException>();
    }

    [Fact]
    public async Task ListAsync_returns_all_users()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        database.Users.Add(CreateUser());
        database.Users.Add(CreateUser("bob", "bob@example.com"));
        await database.SaveChangesAsync();

        var result = await sut.ListAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateUsernameAsync_changes_username()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var user = CreateUser();
        database.Users.Add(user);
        await database.SaveChangesAsync();

        var request = new UpdateUsernameRequest("alice2");

        var result = await sut.UpdateUsernameAsync(user.Id, request);

        result.UserName.Should().Be("alice2");
    }

    [Fact]
    public async Task UpdateUsernameAsync_throws_when_email_belongs_to_another_user()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var user1 = CreateUser();
        var user2 = CreateUser("bob", "bob@example.com");

        database.Users.Add(user1);
        database.Users.Add(user2);
        await database.SaveChangesAsync();

        var request = new UpdateUsernameRequest("bob");

        await sut
            .Invoking(s => s.UpdateUsernameAsync(user1.Id, request))
            .Should()
            .ThrowAsync<DuplicateUserException>();
    }

    [Fact]
    public async Task DeleteAsync_remove_user()
    {
        using var database = CreateContext();
        var sut = CreateSut(database);

        var user = CreateUser();
        database.Users.Add(user);
        await database.SaveChangesAsync();

        await sut.DeleteAsync(user.Id);

        var exists = await database.Users.AnyAsync(u => u.Id == user.Id);
        exists.Should().BeFalse();
    }
}
