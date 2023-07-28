using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityJwt.Api.RequestDTOs;
using SecurityJwt.Api.ResponseDTOs;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Application.IServices;
using SecurityJwt.Domain.Common;
using SecurityJwt.Domain.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SecurityJwt.Api.Controllers.v1;

[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfig _jwtConfig;
    //private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IUnitOfWork unitOfWork,
        UserManager<IdentityUser> userManager,
        IOptionsMonitor<JwtConfig> optionsMonitor,
        //TokenValidationParameters tokenValidationParameters,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService
    ) : base(unitOfWork)
    {
        _userManager = userManager;
        _jwtConfig = optionsMonitor.CurrentValue;
       // _tokenValidationParameters = tokenValidationParameters;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
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
            return Ok(response);
        }

        if(!passwordRegex.IsMatch(request.Password))
        {
            response.ErrorMessage = "Passwords not validated.";
            return Ok(response);
        }

        if (request.Password != request.ConfirmPassword)
        {
            response.ErrorMessage = "Passwords do not match.";
            return Ok(response);
        }

        var userExist = await _unitOfWork.Users.CheckIfUserExistByEmail(request.Email);
        if (userExist)
        {
            response.ErrorMessage = "Invalid request.";
            return Ok(response);
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
            return Ok(response);
        }

        var user = new User
        {
            IdentityUserId = new Guid(identityUser.Id),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Status = true,
            DateCreated = DateTime.UtcNow,
            DateOfBirth = DateTime.UtcNow,
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
            return Ok(response); 
        }

        var identityUser = await _userManager.FindByEmailAsync(request.Email);
        if (identityUser is null)
        {
            response.ErrorMessage = "Email or password is not correct.";
            return Ok(response);
        }

        var user = await _unitOfWork.Users.GetUserByIdentityId(new Guid(identityUser.Id));
        if (user == null)
        {
            response.ErrorMessage = "Email or password is not correct.";
            return Ok(response);
        }
        if (user.Status == false) 
        {
            response.ErrorMessage = "Email or password is not correct.";
            return Ok(response);
        }

        var isVerified = await _userManager.CheckPasswordAsync(identityUser, request.Password);
        if (!isVerified)
        {
            response.ErrorMessage = "Email or password is not correct.";
            return Ok(response);
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


    // Post -> Generate new jwt token and refresh token
    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var response = new RefreshTokenResponseDto();
        response.IsSuccess = false;
        response.RefreshToken = "";
        response.JwtToken = "";

        if (!ModelState.IsValid)
        {
            response.ErrorMessage = "Invalid request";
            return BadRequest(response);
        }

        var isVerified = await VerifyToken(request);
        if (!isVerified.IsSuccess)
        {
            response.ErrorMessage = isVerified.ErrorMessage;
            return BadRequest(response);
        }

        response.IsSuccess = true;
        response.ErrorMessage = "";
        response.JwtToken = isVerified.JwtToken;
        response.RefreshToken = isVerified.RefreshToken;
 
        return Ok(response);
    }


    // verify token 
    private async Task<RefreshTokenResponseDto> VerifyToken(RefreshTokenRequestDto request)
    {
        var response = new RefreshTokenResponseDto();
        response.IsSuccess = false;
        response.JwtToken = "";
        response.RefreshToken = "";

        // if any field is null, then retun false
        if(request.JwtToken is "" || request.RefreshToken is "")
        {
            response.ErrorMessage = "Invalid request";
            return response;
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, // TODO: will be set to true
                ValidateAudience = false, // TODO: will be set to true
                RequireExpirationTime = false, // TODO: will be set to true
                ValidateLifetime = false,
            };

            // check validity of the token
            var principal = tokenHandler.ValidateToken(request.JwtToken, tokenValidationParameters, out var validateToken);

            // if the provided token is not a jwt token
            if(validateToken is JwtSecurityToken jwtSecurityToken)
            {
                var verifyAlgorithm = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if(verifyAlgorithm is false)
                {
                    response.ErrorMessage = "Invalid token.";
                    return response;
                }
            }
            else
            {
                response.ErrorMessage = "Invalid token.";
                return response;
            }

            // check jwt token expiry date
            var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

            if(expiryDate > DateTime.UtcNow)
            {
                response.IsSuccess = true;
                response.ErrorMessage = "";
                response.RefreshToken = request.RefreshToken;
                response.JwtToken = request.JwtToken;
                return response;
            }

            // check if the refresh token is present in database
            var refreshTokenExist = await _unitOfWork.RefreshTokens.GetByRefreshToken(request.RefreshToken);
            if(refreshTokenExist is null)
            {
                response.ErrorMessage = "Invalid token";
                return response;
            }

            // check refresh token expiry date
            if(refreshTokenExist.DateExpire < DateTime.UtcNow)
            {
                response.ErrorMessage = "Expired token";
                return response;
            }

            // check if the refresh token is already used
            if (refreshTokenExist.IsUsed)
            {
                response.ErrorMessage = "Token has been used earlier";
                return response;
            }

            // Check if paired with correct jwt token
            if(refreshTokenExist.JwtToken != request.JwtToken)
            {
                response.ErrorMessage = "Invalid token";
                return response;
            }

            // check if the right user is requesting

            //var currentEmail = await _currentUserService.GetCurrentUserEmail();
            var token = tokenHandler.ReadJwtToken(request.JwtToken);
            var currentEmail = token.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            var currentUser = await _userManager.FindByEmailAsync(currentEmail);

            // if all checking is passed, then process a new token
            var dbUser = await _unitOfWork.Users.GetUserByIdentityId(new Guid(currentUser.Id));
            var newJwtToken = GenerateJwtToken(currentUser);
            var newRefreshToken = new RefreshToken
            {
                Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}",
                JwtToken = newJwtToken,
                IsUsed = false,
                Status = true,
                DateExpire = DateTime.UtcNow.AddMonths(1),
                IdentityUserId = currentUser.Id.ToString()
            };

            var isMarked = await _unitOfWork.RefreshTokens.MarkTokenAsUsed(refreshTokenExist.Id);
            var isUsedTokensDeleted = await _unitOfWork.RefreshTokens.DeleteUsedTokens(new Guid(currentUser.Id));
            var isAdded = await _unitOfWork.RefreshTokens.AddEntity(newRefreshToken);
            if(isMarked && isAdded)
            {
                try
                {
                    await _unitOfWork.CompleteAsync();
                    response.IsSuccess = true;
                    response.ErrorMessage = "";
                    response.JwtToken = newJwtToken;
                    response.RefreshToken = newRefreshToken.Token;

                    return response;
                }
                catch(Exception ex)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = ex.Message;
                    response.JwtToken = "";
                    response.RefreshToken = "";
                    return response;
                } 
            }

            response.IsSuccess = false;
            response.ErrorMessage = "Error processing your request.";
            response.JwtToken = "";
            response.RefreshToken = "";

            return response;
        }
        catch(Exception ex)
        {
            response.ErrorMessage = ex.Message;
            return response;
        }
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
            Expires = DateTime.UtcNow.AddMinutes(2),
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

    // converting a long int into actual datetime
    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
        return dateTime;
    }
}
