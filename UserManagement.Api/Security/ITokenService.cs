using UserManagement.Api.Domain;

namespace UserManagement.Api.Security;

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);
}
