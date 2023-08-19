using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
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

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineOptions : OAuthOptions
    {
        // Constructors
        public LineOptions()
        {
            // Options
            this.CallbackPath = new PathString("/signin-line");
            this.ClaimsIssuer = LineDefaults.AuthenticationScheme;
            this.AuthorizationEndpoint = LineDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = LineDefaults.TokenEndpoint;
            this.UserInformationEndpoint = LineDefaults.UserInformationEndpoint;
            this.VerifyEndpoint = LineDefaults.VerifyEndpoint;

            // Scope
            this.Scope.Add("profile");
            this.Scope.Add("openid");
            this.Scope.Add("email");

            // AccessTokenIdentityClaimActions
            this.AccessTokenIdentityClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "userId");
            this.AccessTokenIdentityClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");

            // IdTokenIdentityClaimActions
            this.IdTokenIdentityClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            this.IdTokenIdentityClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            this.IdTokenIdentityClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        }


        // Properties
        public string VerifyEndpoint { get; set; }

        public ClaimActionCollection AccessTokenIdentityClaimActions { get; } = new ClaimActionCollection();

        public ClaimActionCollection IdTokenIdentityClaimActions { get; } = new ClaimActionCollection();
    }
}
