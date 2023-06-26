﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Persistence.Common
{
    public interface IBaseRepository
    {
        Task<bool> Add<T>(T entity);
        Task<bool> Delete<T>(T entity);
        Task<bool> Update<T>(T entity);
    }
}
