namespace UserManagement.Api.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public Role Role { get; private set; } = default;
    public DateTime CreateAt { get; private set; }
    public DateTime? UpdateAt { get; private set; } = null;

    private User() { }

    public static User Create(
            string userName,
            string email,
            string passwordHash,
            Role role
            ) => new()
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Email = email,
                PasswordHash = passwordHash,
                Role = role,
                CreateAt = DateTime.UtcNow
            };

    public void UpdateUsername(string userName)
    {
        UserName = userName;
        UpdateAt = DateTime.UtcNow;
    }


    public void UpdateEmail(string email)
    {
        Email = email;
        UpdateAt = DateTime.UtcNow;
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdateAt = DateTime.UtcNow;
    }

    public void AssignRole(Role newRole)
    {
        Role = newRole;
        UpdateAt = DateTime.UtcNow;
    }
}
