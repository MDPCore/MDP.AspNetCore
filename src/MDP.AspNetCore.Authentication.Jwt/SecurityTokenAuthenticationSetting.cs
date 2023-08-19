using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Jwt
{
    public class SecurityTokenAuthenticationSetting
    {
        // Properties
        public string Scheme { get; set; } = "JwtBearer";

        public string Header { get; set; } = "Authorization";

        public string Prefix { get; set; } = "Bearer ";

        public string Issuer { get; set; } = String.Empty;

        public string SignKey { get; set; } = String.Empty;
    }
}
