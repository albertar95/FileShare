﻿using Application.Model;
using Application.Persistence.Common;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence
{
    public interface IUserRepository : IBaseRepository
    {
        Task<List<User>> GetUsers(bool IncludeDisabled = true,int Skip = 0,int PageSize = 100);
        Task<User> GetUserById(Guid Id);
        Task<Tuple<byte,User>> LoginUser(string Username,string Password);
    }
}
