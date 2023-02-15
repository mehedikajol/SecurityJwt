namespace SecurityJwt.Api.ResponseDTOs.Generic;

public class AuthResultDto
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
