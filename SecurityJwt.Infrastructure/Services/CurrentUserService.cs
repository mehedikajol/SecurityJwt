using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SecurityJwt.Application.IServices;
using SecurityJwt.Infrastructure.DbContext;
using System.Security.Claims;

namespace SecurityJwt.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public async Task<string> GetCurrentUserEmail()
    {
        if(_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var claims = user.Claims.ToList();
            var email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
            return email;
        }
        return null;
    }

    public async Task<string> GetCurrentUserId()
    {
        if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var claims = user.Claims.ToList();
            var userId = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Id).Value;
            return userId;
        }
        return null;
    }
}
