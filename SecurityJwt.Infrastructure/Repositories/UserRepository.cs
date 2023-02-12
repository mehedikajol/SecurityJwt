using Microsoft.Extensions.Logging;
using SecurityJwt.Application.Repositories;
using SecurityJwt.Domain.Entities;
using SecurityJwt.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger logger) : base(context, logger)
    {
    }
}
