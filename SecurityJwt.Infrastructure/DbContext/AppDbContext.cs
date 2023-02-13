using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecurityJwt.Domain.Entities;

namespace SecurityJwt.Infrastructure.DbContext;

public class AppDbContext : IdentityDbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
	{ }

    // all database table --> excepts the seven tables provided by asp.net core
	public DbSet<User> Users { get; set; }

}
