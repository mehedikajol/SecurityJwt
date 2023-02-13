using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SecurityJwt.Application.IServices;
using SecurityJwt.Infrastructure.DbContext;

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
        var user = _httpContextAccessor.HttpContext.User;
        if (user is null) return null;
        var email = user.Identity.Name;
        return email;
    }

    public async Task<string> GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext.User;
        if (user is null) return null;
        var email = user.Identity.Name;
        var userId = _context.Users.Where(x => x.Email == email).Select(x => x.Id).FirstOrDefaultAsync();
        return userId.ToString();
    }
}
