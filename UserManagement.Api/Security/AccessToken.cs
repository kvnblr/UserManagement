namespace UserManagement.Api.Security;

public record AccessToken(string Token, DateTime ExpiredAtUtc);
