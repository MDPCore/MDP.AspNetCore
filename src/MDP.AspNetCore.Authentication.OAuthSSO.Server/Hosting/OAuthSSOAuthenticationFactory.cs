using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public class OAuthSSOAuthenticationFactory : ServiceFactory<WebApplicationBuilder, OAuthSSOAuthenticationFactory.Setting>
    {
        // Constructors
        public OAuthSSOAuthenticationFactory() : base("Authentication", "OAuthSSO.Server", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, OAuthSSOAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentNullException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentNullException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (setting.ClientCredentials == null) throw new InvalidOperationException($"{nameof(setting.ClientCredentials)}=null");

            // AuthenticationControllerSetting
            applicationBuilder.Services.TryAddSingleton(serviceProvider =>
            {
                // Create
                var authenticationControllerSetting = new AuthenticationControllerSetting();
                authenticationControllerSetting.JwtTokenName = setting.JwtTokenName;
                authenticationControllerSetting.AccessTokenExpireMinutes = setting.AccessTokenExpireMinutes;
                authenticationControllerSetting.RefreshTokenExpireMinutes = setting.RefreshTokenExpireMinutes;
                authenticationControllerSetting.ClientCredentialList = setting.ClientCredentials.ToList();

                // Return
                return authenticationControllerSetting;
            });
        }


        // Class
        public class Setting
        {
            // Properties
            public string JwtTokenName { get; set; } = "JwtBearer";

            public int AccessTokenExpireMinutes { get; set; } = 30; // 30 minutes

            public int RefreshTokenExpireMinutes { get; set; } = 20160; // 14 days

            public List<ClientCredential> ClientCredentials { get; set; }
        }
    }
}