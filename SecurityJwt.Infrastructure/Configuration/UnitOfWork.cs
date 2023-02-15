using Microsoft.Extensions.Logging;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Application.IRepositories;
using SecurityJwt.Application.IServices;
using SecurityJwt.Infrastructure.DbContext;
using SecurityJwt.Infrastructure.Repositories;

namespace SecurityJwt.Infrastructure.Configuration;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    // all repositories
    public IUserRepository Users { get; private set; }
    public IRefreshTokenRepository RefreshTokens { get; set; }

    public UnitOfWork(
        AppDbContext context, 
        ILoggerFactory logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger.CreateLogger("db_logs");

        Users = new UserRepository(context, _logger, currentUserService);
        RefreshTokens = new RefreshTokenRepository(context, _logger);
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
