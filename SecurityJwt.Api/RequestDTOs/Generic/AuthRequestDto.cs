using System.ComponentModel.DataAnnotations;

namespace SecurityJwt.Api.RequestDTOs.Generic;

public class AuthRequestDto
{
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
