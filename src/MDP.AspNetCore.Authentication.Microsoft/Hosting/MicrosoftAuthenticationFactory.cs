using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Microsoft
{
    public class MicrosoftAuthenticationFactory : ServiceFactory<WebApplicationBuilder, MicrosoftAuthenticationFactory.Setting>
    {
        // Constructors
        public MicrosoftAuthenticationFactory() : base("Authentication", "Microsoft", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, MicrosoftAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.ClientId) == true) throw new InvalidOperationException($"{nameof(setting.ClientId)}=null");
            if (string.IsNullOrEmpty(setting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(setting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // MicrosoftAuthentication
            authenticationBuilder.AddMicrosoft(options =>
            {
                // Options
                options.TenantId = "consumers";
                options.ClientId = setting.ClientId;
                options.ClientSecret = setting.ClientSecret;
                options.CallbackPath = new PathString("/.auth/login/microsoft/callback");

                // Options.SignIn
                options.ConfigureSignIn();
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public string ClientId { get; set; } = string.Empty;

            public string ClientSecret { get; set; } = string.Empty;
        }
    }
}