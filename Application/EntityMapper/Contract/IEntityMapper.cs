﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityMapper.Contract
{
    public interface IEntityMapper
    {
        T EntityMap<T>(object source,T input = null) where T : class;
    }
}
