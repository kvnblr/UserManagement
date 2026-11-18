using System.ComponentModel.DataAnnotations;

namespace UserManagement.Api.DataTransferObjects;

public record ChangePasswordRequest(
        [Required]
        string password,
        [Required]
        [StringLength (100, MinimumLength = 8)]
        string newPassword);
