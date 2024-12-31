using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Apple
{
    public class AppleAuthenticationFactory : ServiceFactory<WebApplicationBuilder, AppleAuthenticationFactory.Setting>
    {
        // Constructors
        public AppleAuthenticationFactory() : base("Authentication", "Apple", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AppleAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentNullException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentNullException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.TeamId) == true) throw new InvalidOperationException($"{nameof(setting.TeamId)}=null");
            if (string.IsNullOrEmpty(setting.ServiceId) == true) throw new InvalidOperationException($"{nameof(setting.ServiceId)}=null");
            if (string.IsNullOrEmpty(setting.KeyId) == true) throw new InvalidOperationException($"{nameof(setting.KeyId)}=null");
            if (string.IsNullOrEmpty(setting.SignKey) == true) throw new InvalidOperationException($"{nameof(setting.SignKey)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // AppleAuthentication
            authenticationBuilder.AddApple(options =>
            {
                // Options
                options.ServiceId = setting.ServiceId;
                options.TeamId = setting.TeamId;
                options.KeyId = setting.KeyId;
                options.SignKey = setting.SignKey;
                options.ClientSecret = options.CreateClientSecret();
                options.CallbackPath = new PathString("/.auth/login/apple/callback");                

                // Options.SignIn
                options.ConfigureSignIn();
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public string ServiceId { get; set; } = string.Empty;

            public string TeamId { get; set; } = string.Empty;

            public string KeyId { get; set; } = string.Empty;

            public string SignKey { get; set; } = string.Empty;
        }
    }
}