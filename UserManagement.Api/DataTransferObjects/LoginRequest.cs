using System.ComponentModel.DataAnnotations;

namespace UserManagement.Api.DataTransferObjects;

public record LoginRequest(
        [Required] string UserNameOrEmail,
        [Required] string Password);
