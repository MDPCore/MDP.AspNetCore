using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public class AuthenticationControllerSetting
    {
        // Properties
        public int ExpireMinutes { get; set; } = 5;

        public string JwtTokenName { get; set; } = "JwtBearer";

        public List<ClientCredential> ClientCredentialList { get; set; } = null;
    }
}
