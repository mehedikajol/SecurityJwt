using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecurityJwt.Api.RequestDTOs;
using SecurityJwt.Api.ResponseDTOs;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Domain.Common;
using SecurityJwt.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecurityJwt.Api.Controllers.v1
{
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
            if (!ModelState.IsValid)
                return BadRequest();

            if (request.Password != request.ConfirmPassword)
                return BadRequest();

            var identityUser = new IdentityUser
            {
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = true,
            };

            var isCreated = await _userManager.CreateAsync(identityUser, request.Password);

            if (!isCreated.Succeeded)
                return BadRequest();

            var user = new User
            {
                IdentityUserId = new Guid(identityUser.Id),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                Status = true,
                DateCreated = DateTime.Now,
                DateOfBirth = DateTime.Now,
            };

            await _unitOfWork.Users.AddEntity(user);
            await _unitOfWork.CompleteAsync();

            var token = GenerateJwtToken(identityUser);
            var response = new UserRegisterResponseDto
            {
                JwtToken = token
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest();

            var identityUser = await _userManager.FindByEmailAsync(request.Email);
            if(identityUser is null) return NotFound();

            var user = await _unitOfWork.Users.GetUserByIdentityId(new Guid(identityUser.Id));
            if (user == null) return NotFound();
            if(user.Status == false) return BadRequest();

            var isVerified = await _userManager.CheckPasswordAsync(identityUser, request.Password);
            if(!isVerified)
                return BadRequest();

            var token = GenerateJwtToken(identityUser);
            var response = new UserLoginResponseDto()
            {
                JwtToken = token
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
    }
}
