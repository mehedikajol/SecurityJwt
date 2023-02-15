using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityJwt.Api.RequestDTOs;
using SecurityJwt.Api.ResponseDTOs;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Domain.Common;
using SecurityJwt.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SecurityJwt.Api.Controllers.v1;

public class AuthController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfig _jwtConfig;

    public AuthController(
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager,
        IOptionsMonitor<JwtConfig> optionsMonitor
    ) : base(unitOfWork)
    {
        _userManager = userManager;
        _jwtConfig = optionsMonitor.CurrentValue;
    }

    // Post --> Create user
    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto request)
    {
        var response = new UserRegisterResponseDto();
        response.IsSuccess = false;
        response.RefreshToken = "";
        response.JwtToken = "";

        var passwordRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");

        if (!ModelState.IsValid)
        {
            response.ErrorMessage = "Invalid request.";
            return BadRequest(response);
        }

        if(!passwordRegex.IsMatch(request.Password))
        {
            response.ErrorMessage = "Passwords not validated.";
            return BadRequest(response);
        }

        if (request.Password != request.ConfirmPassword)
        {
            response.ErrorMessage = "Passwords do not match.";
            return BadRequest(response);
        }

        var userExist = await _unitOfWork.Users.CheckIfUserExistByEmail(request.Email);
        if (userExist)
        {
            response.ErrorMessage = "Invalid request.";
            return BadRequest(response);
        }

        var identityUser = new IdentityUser
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
        };

        var isCreated = await _userManager.CreateAsync(identityUser, request.Password);

        if (!isCreated.Succeeded)
        {
            response.ErrorMessage = "Error processing you request.";
            return BadRequest(response);
        }

        var user = new User
        {
            IdentityUserId = new Guid(identityUser.Id),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = true,
            DateCreated = DateTime.Now,
            DateOfBirth = DateTime.Now,
        };

        await _unitOfWork.Users.AddEntity(user);

        var token = GenerateJwtToken(identityUser);
        var refreshToken = new RefreshToken
        {
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
            JwtToken = token,
            IsUsed = false,
            Status = true,
            DateExpire = DateTime.UtcNow.AddMonths(1),
            IdentityUserId = identityUser.Id.ToString()
        };
        await _unitOfWork.RefreshTokens.AddEntity(refreshToken);

        await _unitOfWork.CompleteAsync();

        response.IsSuccess = true;
        response.ErrorMessage = "";
        response.JwtToken = token;
        response.RefreshToken = refreshToken.Token;

        return Ok(response);
    }


    // Post -> User login
    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
    {
        var response = new UserLoginResponseDto();
        response.IsSuccess = false;
        response.RefreshToken = "";
        response.JwtToken = "";

        if (!ModelState.IsValid) 
        {
            response.ErrorMessage = "Invalid request.";
            return BadRequest(response); 
        }

        var identityUser = await _userManager.FindByEmailAsync(request.Email);
        if (identityUser is null)
        {
            response.ErrorMessage = "Email or password is not correct.";
            return NotFound(response);
        }

        var user = await _unitOfWork.Users.GetUserByIdentityId(new Guid(identityUser.Id));
        if (user == null)
        {
            response.ErrorMessage = "Email or password is not correct.";
            return NotFound(response);
        }
        if (user.Status == false) 
        {
            response.ErrorMessage = "Email or password is not correct.";
            return BadRequest(response);
        }

        var isVerified = await _userManager.CheckPasswordAsync(identityUser, request.Password);
        if (!isVerified)
        {
            response.ErrorMessage = "Email or password is not correct.";
            return BadRequest(response);
        }

        var token = GenerateJwtToken(identityUser);
        var refreshToken = new RefreshToken
        {
            Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
            JwtToken = token,
            IsUsed = false,
            Status = true,
            DateExpire = DateTime.UtcNow.AddMonths(1),
            IdentityUserId = identityUser.Id.ToString()
        };

        await _unitOfWork.RefreshTokens.AddEntity(refreshToken);
        await _unitOfWork.CompleteAsync();

        response.IsSuccess = true;
        response.ErrorMessage = "";
        response.JwtToken = token;
        response.RefreshToken = refreshToken.Token;

        return Ok(response);
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var response = new RefreshTokenResponseDto();

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }


        response = new RefreshTokenResponseDto()
        {
            JwtToken = request.JwtToken,
            RefreshToken = request.RefreshToken
        };

        return Ok(response);
    }


    // generate jwt token
    private string GenerateJwtToken(IdentityUser user)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // will be used bt the refresh token
            }),
            Expires = DateTime.UtcNow.AddMonths(6),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = jwtHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtHandler.WriteToken(token);

        return jwtToken;
    }

    // Random string generator
    private string RandomStringGenerator(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
