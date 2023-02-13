using SecurityJwt.Application.IRepositories;

namespace SecurityJwt.Application.IConfiguration;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    Task CompleteAsync();
}
