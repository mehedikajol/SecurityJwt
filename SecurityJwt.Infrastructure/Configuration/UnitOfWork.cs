using Microsoft.Extensions.Logging;
using SecurityJwt.Application.IConfiguration;
using SecurityJwt.Application.Repositories;
using SecurityJwt.Infrastructure.DbContext;
using SecurityJwt.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Infrastructure.Configuration;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    // all repositories
    public IUserRepository Users { get; private set; }

    public UnitOfWork(
        AppDbContext context, 
        ILoggerFactory logger)
    {
        _context = context;
        _logger = logger.CreateLogger("db_logs");

        Users = new UserRepository(context, _logger);
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
