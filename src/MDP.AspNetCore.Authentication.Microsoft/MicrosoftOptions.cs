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

namespace MDP.AspNetCore.Authentication.Microsoft
{
    public class MicrosoftOptions : OAuthOptions
    {
        // Fields
        private string _tenantId = null;


        // Constructors
        public MicrosoftOptions()
        {
            // Options
            this.CallbackPath = new PathString("/signin-azuread");
            this.AuthorizationEndpoint = MicrosoftDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = MicrosoftDefaults.TokenEndpoint;
            this.UserInformationEndpoint = MicrosoftDefaults.UserInformationEndpoint;
            this.TenantId = "common";

            // Scope
            this.Scope.Add("openid");
            this.Scope.Add("email");
            this.Scope.Add("profile");
            this.Scope.Add("User.Read");

            // Claim
            this.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
            this.ClaimActions.MapJsonKey(ClaimTypes.Name, "displayName");
            this.ClaimActions.MapJsonKey(ClaimTypes.Email, "mail");
        }


        // Properties
        public string TenantId
        {
            get
            {
                // Get
                return _tenantId;
            }
            set
            {
                // Set
                _tenantId = value;
                this.AuthorizationEndpoint = string.Format(MicrosoftDefaults.AuthorizationEndpoint, value);
                this.TokenEndpoint = string.Format(MicrosoftDefaults.TokenEndpoint, value);
            }
        }
    }
}
