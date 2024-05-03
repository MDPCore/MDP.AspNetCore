using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.OAuthSSO
{
    public class OAuthSSOAuthenticationFactory : ServiceFactory<WebApplicationBuilder, OAuthSSOAuthenticationFactory.Setting>
    {
        // Constructors
        public OAuthSSOAuthenticationFactory() : base("Authentication", "OAuthSSO", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, OAuthSSOAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.ClientId) == true) throw new InvalidOperationException($"{nameof(setting.ClientId)}=null");
            
            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // OAuthSSOAuthentication
            authenticationBuilder.AddOAuthSSO(options =>
            {
                // Options
                options.ClientId = setting.ClientId;
                options.ServerUrl = setting.ServerUrl;
                options.CallbackPath = new PathString("/.auth/login/oauthsso/callback");
                options.UsePkce = true;

                // Options.SignIn
                options.ConfigureSignIn();
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public string ClientId { get; set; } = string.Empty;

            public string ServerUrl { get; set; } = string.Empty;
        }
    }
}