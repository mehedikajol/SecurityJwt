using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecurityJwt.Application.IRepositories;
using SecurityJwt.Application.IServices;
using SecurityJwt.Domain.Entities;
using SecurityJwt.Infrastructure.DbContext;

namespace SecurityJwt.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ICurrentUserService _currentUserService;
    public UserRepository(AppDbContext context, ILogger logger, ICurrentUserService currentUserService) : base(context, logger)
    {
        _currentUserService = currentUserService;
    }

    public async Task<bool> CheckIfUserExistByEmail(string email)
    {
        var user = await _dbSet.Where(u => u.Email == email).FirstOrDefaultAsync();
        if (user == null) 
        { 
            return false;
        }
        return true;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _dbSet.Where(x => x.Email== email).FirstOrDefaultAsync();
        return user;
    }

    public async Task<User> GetUserByIdentityId(Guid id)
    {
        var user = await _dbSet.Where(u => u.IdentityUserId == id).FirstOrDefaultAsync();
        if (user is null)
        {
            return null;
        }
        return user;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var email = await _currentUserService.GetCurrentUserEmail();
        if (email is null)
        {
            return false;
        }

        var dbUser = await _dbSet.Where(x => x.Email == email).FirstOrDefaultAsync();

        if (dbUser is null)
            return false;

        dbUser.FirstName = user.FirstName;
        dbUser.LastName = user.LastName;
        dbUser.PhoneNumber = user.PhoneNumber;
        dbUser.DateOfBirth = user.DateOfBirth == DateTime.MinValue ? dbUser.DateOfBirth : user.DateOfBirth;

        return true;
    }
}
