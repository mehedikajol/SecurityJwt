using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecurityJwt.Domain.Entities;

namespace SecurityJwt.Infrastructure.DbContext;

public class AppDbContext : IdentityDbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
	{ }

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		foreach(var entry in ChangeTracker.Entries<BaseEntity>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.DateCreated = DateTime.UtcNow;
					break;

				case EntityState.Modified:
					entry.Entity.DateModified = DateTime.UtcNow;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
        return base.SaveChangesAsync(cancellationToken);
    }

    // all database table --> excepts the seven tables provided by asp.net core
	public DbSet<User> Users { get; set; }

}
