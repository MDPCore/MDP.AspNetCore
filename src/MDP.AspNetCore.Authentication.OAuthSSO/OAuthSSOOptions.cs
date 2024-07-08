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

namespace MDP.AspNetCore.Authentication.OAuthSSO
{
    public class OAuthSSOOptions : OAuthOptions
    {
        // Fields
        private string _serverUrl = null;


        // Constructors
        public OAuthSSOOptions()
        {
            // Options
            this.ClientSecret = Guid.NewGuid().ToString();
            this.CallbackPath = new PathString("/signin-oauthsso");
            this.AuthorizationEndpoint = OAuthSSODefaults.AuthorizationEndpoint;
            this.LogoutEndpoint = OAuthSSODefaults.LogoutEndpoint;
            this.TokenEndpoint = OAuthSSODefaults.TokenEndpoint;
            this.UserInformationEndpoint = OAuthSSODefaults.UserInformationEndpoint;

            // Scope
            this.Scope.Add("profile");
            this.Scope.Add("openid");
            this.Scope.Add("email");

            // Claim
            this.ClaimActions.Clear();
        }


        // Properties
        public string ServerUrl
        {
            get
            {
                // Get
                return _serverUrl;
            }
            set
            {
                // Set
                _serverUrl = value;
                this.AuthorizationEndpoint = string.Format(OAuthSSODefaults.AuthorizationEndpoint, value);
                this.LogoutEndpoint = string.Format(OAuthSSODefaults.LogoutEndpoint, value);
                this.TokenEndpoint = string.Format(OAuthSSODefaults.TokenEndpoint, value);
                this.UserInformationEndpoint = string.Format(OAuthSSODefaults.UserInformationEndpoint, value);
            }
        }

        public string LogoutEndpoint { get; private set; }
    }
}
