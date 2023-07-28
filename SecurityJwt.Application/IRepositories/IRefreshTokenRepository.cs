using SecurityJwt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Application.IRepositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<bool> MarkTokenAsUsed(Guid id);
    Task<RefreshToken> GetByRefreshToken(string refreshToken);
    Task<bool> DeleteUsedTokens(Guid userId);
}
