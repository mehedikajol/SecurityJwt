﻿using SecurityJwt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> GetUserByIdentityId(Guid id);
}
