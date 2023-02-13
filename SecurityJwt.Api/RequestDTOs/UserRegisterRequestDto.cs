using SecurityJwt.Api.RequestDTOs.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecurityJwt.Api.RequestDTOs;

public class UserRegisterRequestDto : AuthRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}