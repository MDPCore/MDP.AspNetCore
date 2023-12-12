using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Google
{
    public class GoogleAuthenticationFactory : Factory<WebApplicationBuilder, GoogleAuthenticationSetting>
    {
        // Constructors
        public GoogleAuthenticationFactory() : base("Authentication", "Google") { }


        // Methods
        public override List<ServiceRegistration> ConfigureService(WebApplicationBuilder webApplicationBuilder, GoogleAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddGoogleAuthentication
            webApplicationBuilder.Services.AddGoogleAuthentication(authenticationSetting);

            // Return
            return null;
        }
    }
}