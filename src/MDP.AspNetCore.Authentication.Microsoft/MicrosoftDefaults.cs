using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Microsoft
{
    public class MicrosoftDefaults
    {
        // Constants
        public static readonly string AuthenticationScheme = "Microsoft";

        public static readonly string AuthorizationEndpoint = "https://login.microsoftonline.com/{0}/oauth2/v2.0/authorize";

        public static readonly string TokenEndpoint = "https://login.microsoftonline.com/{0}/oauth2/v2.0/token";

        public static readonly string UserInformationEndpoint = "https://graph.microsoft.com/v1.0/me";
    }
}