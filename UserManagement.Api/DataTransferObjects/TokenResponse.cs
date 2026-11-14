namespace UserManagement.Api.DataTransferObjects;

public record TokenResponse(
        string AccessToken,
        DateTime ExpiredAtUtc,
        UserResponse User);
