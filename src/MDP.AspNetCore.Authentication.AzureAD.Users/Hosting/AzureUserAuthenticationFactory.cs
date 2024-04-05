using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    public class AzureUserAuthenticationFactory : ServiceFactory<WebApplicationBuilder, AzureUserAuthenticationFactory.Setting>
    {
        // Constructors
        public AzureUserAuthenticationFactory() : base("Authentication", "AzureAD.Users", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AzureUserAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.TenantId) == true) throw new InvalidOperationException($"{nameof(setting.TenantId)}=null");
            if (string.IsNullOrEmpty(setting.ClientId) == true) throw new InvalidOperationException($"{nameof(setting.ClientId)}=null");
            if (string.IsNullOrEmpty(setting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(setting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // AzureUserAuthentication
            authenticationBuilder.AddAzureUser(options =>
            {
                // Options
                options.TenantId = setting.TenantId; 
                options.ClientId = setting.ClientId;
                options.ClientSecret = setting.ClientSecret;
                options.CallbackPath = new PathString("/.auth/login/aad/users/callback");

                // Options.SignIn
                options.ConfigureSignIn();
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public string TenantId { get; set; } = string.Empty;

            public string ClientId { get; set; } = string.Empty;

            public string ClientSecret { get; set; } = string.Empty;
        }
    }
}