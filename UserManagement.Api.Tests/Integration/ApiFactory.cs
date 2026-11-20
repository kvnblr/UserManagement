using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Api.Data;
using UserManagement.Api.Domain;
using UserManagement.Api.Security;

namespace UserManagement.Api.Tests.Integration;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    public const string AdminUserName = "admin";
    public const string AdminEmail = "admin@example.com";
    public const string AdminPassword = "Admin123!";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = "DataSource=:memory:",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:Key"] = "test-key-that-is-long-enough-for-hmacsha256-1234567890"
            });
        });

        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                        d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(_connection));
        });
    }

    public void InitializeDatabase()
    {
        using var scope = Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (!context.Users.Any(u => u.UserName == AdminUserName))
        {
            context.Users.Add(User.Create(
                    AdminUserName,
                    AdminEmail,
                    hasher.Hash(AdminPassword),
                    Role.Admin));

            context.SaveChanges();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();
        }
    }
}
