﻿using SecurityJwt.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityJwt.Application.IConfiguration;

public interface IUnitOfWork
{
    IUserRepository Users { get; }

    Task CompleteAsync();
}
