﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileShareApi.Auth.Contract
{
    public interface IAccessControl
    {
        bool Check(HttpRequestMessage request);
        string GetClientIp(HttpRequestMessage request = null);
    }
}
