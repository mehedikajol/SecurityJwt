using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecurityJwt.Application.IRepositories;
using SecurityJwt.Domain.Entities;
using SecurityJwt.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }

    public async Task<RefreshToken> GetByRefreshToken(string refreshToken)
    {
        return await _dbSet.Where(x => x.Token == refreshToken).FirstOrDefaultAsync();
    }

    public async Task<bool> MarkTokenAsUsed(Guid id)
    {
        var token = await _dbSet.FindAsync(id);
        if (token == null)
        {
            return false;
        }

        token.IsUsed = true;
        return true;
    }
}
