using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Jwt
{
    public class SecurityTokenAuthenticationFactory : Factory<WebApplicationBuilder, SecurityTokenAuthenticationSetting>
    {
        // Constructors
        public SecurityTokenAuthenticationFactory() : base("Authentication", "Jwt") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, SecurityTokenAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddSecurityTokenAuthentication
            webApplicationBuilder.Services.AddSecurityTokenAuthentication(authenticationSetting);
        }
    }
}
