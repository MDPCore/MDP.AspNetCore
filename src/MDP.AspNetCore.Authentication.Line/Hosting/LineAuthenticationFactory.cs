using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineAuthenticationFactory : ServiceFactory<WebApplicationBuilder, LineAuthenticationFactory.Setting>
    {
        // Constructors
        public LineAuthenticationFactory() : base("Authentication", "Line", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, LineAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentNullException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentNullException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.ClientId) == true) throw new InvalidOperationException($"{nameof(setting.ClientId)}=null");
            if (string.IsNullOrEmpty(setting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(setting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // LineAuthentication
            authenticationBuilder.AddLine(options =>
            {
                // Options
                options.ClientId = setting.ClientId;
                options.ClientSecret = setting.ClientSecret;

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

