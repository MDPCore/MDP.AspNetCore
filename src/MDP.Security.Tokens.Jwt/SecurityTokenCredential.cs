using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Security.Tokens.Jwt
{
    public class SecurityTokenCredential
    {
        // Properties
        public string Name { get; set; } = null;

        public string Algorithm { get; set; } = null;

        public string SignKey { get; set; } = null;

        public string Issuer { get; set; } = null;

        public int ExpireMinutes { get; set; } = 30;
    }
}
