using Application.Helper;
using FileShareApi.Auth.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace FileShareApi.Auth.Service
{
    public class AccessControl : IAccessControl
    {
        private readonly IEncryptionHelper _encryptionHelper;
        public AccessControl(IEncryptionHelper encryptionHelper)
        {
            _encryptionHelper = encryptionHelper;
        }
        public bool Check(HttpRequestMessage request)
        {
            bool result = false;
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var clientIp = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
                if(request.Headers.Any(k => k.Key == "accessToken"))
                {
                    var token = request.Headers.FirstOrDefault(k => k.Key == "accessToken").Value.FirstOrDefault() ?? "";
                    if (request.Headers.Any(k => k.Key == "ts"))
                    {
                        var timestamp = request.Headers.FirstOrDefault(k => k.Key == "ts").Value.FirstOrDefault() ?? "";
                        result = _encryptionHelper.AccessCheckByToken(token, clientIp,timestamp);
                    }
                }
            }
            return result;
        }
    }
}