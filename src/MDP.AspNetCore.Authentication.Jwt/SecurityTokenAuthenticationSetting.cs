using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Jwt
{
    public class SecurityTokenAuthenticationSetting
    {
        // Properties
        public List<SecurityTokenAuthenticationCredential> Credentials { get; set; } = new List<SecurityTokenAuthenticationCredential>();
    }
}
