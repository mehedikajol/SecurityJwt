﻿namespace SecurityJwt.Api.RequestDTOs;

public class RefreshTokenRequestDto
{
    public string JwtToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
