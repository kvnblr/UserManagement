namespace UserManagement.Api.DataTransferObjects;

public record UserResponse(
    Guid Id,
    string UserName,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime UpdatedAt
        );
