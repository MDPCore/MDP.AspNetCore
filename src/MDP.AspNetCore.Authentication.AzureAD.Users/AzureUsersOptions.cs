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

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    public class AzureUsersOptions : OAuthOptions
    {
        // Fields
        private string _tenantId = null;


        // Constructors
        public AzureUsersOptions()
        {
            // Options
            this.CallbackPath = new PathString("/signin-aad-users");
            this.AuthorizationEndpoint = AzureUsersDefaults.AuthorizationEndpoint;
            this.TokenEndpoint = AzureUsersDefaults.TokenEndpoint;
            this.UserInformationEndpoint = AzureUsersDefaults.UserInformationEndpoint;
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
                this.AuthorizationEndpoint = string.Format(AzureUsersDefaults.AuthorizationEndpoint, value);
                this.TokenEndpoint = string.Format(AzureUsersDefaults.TokenEndpoint, value);
            }
        }
    }
}
