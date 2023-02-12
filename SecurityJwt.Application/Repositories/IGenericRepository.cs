using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Application.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllEntities();
    Task<IEnumerable<TEntity>> FindEntities(Expression<Func<TEntity, bool>> expression);
    Task<TEntity> GetEntityById(Guid id);

    Task<bool> AddEntity(TEntity entity);
    Task<bool> AddEntities(IEnumerable<TEntity> entities);

    Task<bool> RemoveEntity(TEntity entity);
    Task<bool> RemoveEntities(IEnumerable<TEntity> entities);
}
