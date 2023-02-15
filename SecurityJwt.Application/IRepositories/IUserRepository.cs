using SecurityJwt.Domain.Entities;

namespace SecurityJwt.Application.IRepositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> UpdateUserAsync(User user);
    Task<User> GetUserByIdentityId(Guid id);
    Task<User> GetUserByEmail(string email);
    Task<bool> CheckIfUserExistByEmail(string email);
}
