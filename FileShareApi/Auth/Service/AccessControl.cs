using Application.Helper;
using FileShareApi.Auth.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
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
            string clientIp = GetClientIp(request);
            if (request.Headers.Any(k => k.Key == "accessToken"))
            {
                var token = request.Headers.FirstOrDefault(k => k.Key == "accessToken").Value.FirstOrDefault() ?? "";
                if (request.Headers.Any(k => k.Key == "ts"))
                {
                    var timestamp = request.Headers.FirstOrDefault(k => k.Key == "ts").Value.FirstOrDefault() ?? "";
                    result = _encryptionHelper.AccessCheckByToken(token, clientIp, timestamp);
                }
            }
            return result;
        }
        public string GetClientIp(HttpRequestMessage request = null)
        {
            string ip = "";
            ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }
    }
}