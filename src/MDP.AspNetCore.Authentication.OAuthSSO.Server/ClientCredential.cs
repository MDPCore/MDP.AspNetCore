using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public class ClientCredential
    {
        // Properties
        public string ClientId { get; set; } = null;

        public string ClientSecret { get; set; } = null;

        public string RedirectUri { get; set; } = null;
    }
}
