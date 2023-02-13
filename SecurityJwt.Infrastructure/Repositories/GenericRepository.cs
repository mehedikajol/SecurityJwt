using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecurityJwt.Application.IRepositories;
using SecurityJwt.Infrastructure.DbContext;
using System.Linq.Expressions;

namespace SecurityJwt.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;
    internal DbSet<TEntity> _dbSet;
    protected readonly ILogger _logger;

    public GenericRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllEntities()
    {
        var entities = await _dbSet.ToListAsync();
        return entities;
    }

    public virtual async Task<IEnumerable<TEntity>> FindEntities(Expression<Func<TEntity, bool>> expression)
    {
        var entities = await _dbSet.Where(expression).ToListAsync();
        return entities;
    }

    public virtual async Task<TEntity> GetEntityById(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity;
    }

    public virtual async Task<bool> AddEntities(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return true;
    }

    public virtual async Task<bool> AddEntity(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        return true;
    }

    public virtual async Task<bool> RemoveEntity(TEntity entity)
    {
        _dbSet.Remove(entity);
        return true;
    }

    public virtual async Task<bool> RemoveEntities(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
        return true;
    }
}
