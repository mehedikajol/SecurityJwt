using System.ComponentModel.DataAnnotations;

namespace SecurityJwt.Api.RequestDTOs.Generic;

public class AuthRequestDto
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
