using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.GitHub
{
    public class GitHubDefaults
    {
        // Constants
        public static readonly string AuthenticationScheme = "GitHub";

        public static readonly string AuthorizationEndpoint = "https://github.com/login/oauth/authorize";

        public static readonly string TokenEndpoint = "https://github.com/login/oauth/access_token";

        public static readonly string UserInformationEndpoint = "https://api.github.com/user";
    }
}