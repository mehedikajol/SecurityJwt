using SecurityJwt.Application.IRepositories;

namespace SecurityJwt.Application.IConfiguration;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task CompleteAsync();
}
