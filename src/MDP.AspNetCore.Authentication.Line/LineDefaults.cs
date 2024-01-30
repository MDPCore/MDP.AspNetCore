using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineDefaults
    {
        // Constants
        public static readonly string AuthenticationScheme = "Line";

        public static readonly string AuthorizationEndpoint = "https://access.line.me/oauth2/v2.1/authorize";

        public static readonly string TokenEndpoint = "https://api.line.me/oauth2/v2.1/token";

        public static readonly string UserInformationEndpoint = "https://api.line.me/oauth2/v2.1/verify";
    }
}