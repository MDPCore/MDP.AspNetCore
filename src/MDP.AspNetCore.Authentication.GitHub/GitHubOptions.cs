using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.GitHub
{
    public class GitHubOptions : OAuthOptions
    {
        // Constructors
        public GitHubOptions()
        {
            // Options
            this.CallbackPath = new PathString("/signin-github");
            this.AuthorizationEndpoint = GitHubDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = GitHubDefaults.TokenEndpoint;
            this.UserInformationEndpoint = GitHubDefaults.UserInformationEndpoint;

            // Claim
            this.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            this.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            this.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        }
    }
}
