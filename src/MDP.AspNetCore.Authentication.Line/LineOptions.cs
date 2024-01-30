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
            this.CallbackPath = new PathString("/.auth/login/line/callback");
            this.ClaimsIssuer = LineDefaults.AuthenticationScheme;
            this.AuthorizationEndpoint = LineDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = LineDefaults.TokenEndpoint;
            this.UserInformationEndpoint = LineDefaults.UserInformationEndpoint;

            // Scope
            this.Scope.Add("profile");
            this.Scope.Add("openid");
            this.Scope.Add("email");

            // Claim
            this.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
            this.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
            this.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            this.ClaimActions.MapJsonKey(ProfileClaimTypes.Picture, "picture");
        }
    }
}
