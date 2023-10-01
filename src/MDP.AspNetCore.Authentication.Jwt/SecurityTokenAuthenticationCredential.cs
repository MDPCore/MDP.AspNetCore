using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Jwt
{
    public class SecurityTokenAuthenticationCredential
    {
        // Properties
        public string Scheme { get; set; } = "JwtBearer";

        public string Header { get; set; } = "Authorization";

        public string Prefix { get; set; } = "Bearer";

        public string Algorithm { get; set; } = null;

        public string SignKey { get; set; } = null;

        public string Issuer { get; set; } = null;
    }
}
