using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EsperancaSolidaria.Infra.Security.Models
{
    public class IdentityTokenResponse
    {
        public bool Success { get; private set; }
        public string Error { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        public IdentityTokenResponse(string error)
        {
            Success = false;
            Error = error;
        }

        public IdentityTokenResponse(string accessToken, string refreshToken)
        {
            Success = true;
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}