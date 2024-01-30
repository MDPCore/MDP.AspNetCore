using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Google
{
    public class GoogleAuthenticationFactory : ServiceFactory<WebApplicationBuilder, GoogleAuthenticationSetting>
    {
        // Constructors
        public GoogleAuthenticationFactory() : base("Authentication", "Google") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, GoogleAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddGoogleAuthentication
            webApplicationBuilder.Services.AddGoogleAuthentication(authenticationSetting);
        }
    }
}