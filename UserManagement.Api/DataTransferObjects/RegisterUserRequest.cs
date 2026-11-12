using System.ComponentModel.DataAnnotations;

namespace UserManagement.Api.DataTransferObjects;

public record RegisterUserRequest(
    [Required]
    [StringLength(50, MinimumLength = 3)]
    string Name,
    [Required]
    [EmailAddress]
    string Email,
    [Required]
    [StringLength(100, MinimumLength = 8)]
    string Password
        );
