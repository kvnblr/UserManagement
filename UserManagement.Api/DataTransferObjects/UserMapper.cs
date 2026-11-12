using UserManagement.Api.Domain;

namespace UserManagement.Api.DataTransferObjects;

public static class UserMapper
{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(
             user.Id,
             user.UserName,
             user.Email,
             user.Role.ToString(),
             user.CreateAt,
             user.UpdateAt);
    }
}
