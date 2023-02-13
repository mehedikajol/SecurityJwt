using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecurityJwt.Application.Repositories;
using SecurityJwt.Domain.Entities;
using SecurityJwt.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<User> GetUserByIdentityId(Guid id)
    {
        var user = await _dbSet.FindAsync(id);
        if (user is null)
            return new User();
        return user;
    }
}
