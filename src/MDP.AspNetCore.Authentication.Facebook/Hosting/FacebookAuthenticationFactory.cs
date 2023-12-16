using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Facebook
{
    public class FacebookAuthenticationFactory : Factory<WebApplicationBuilder, FacebookAuthenticationSetting>
    {
        // Constructors
        public FacebookAuthenticationFactory() : base("Authentication", "Facebook") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, FacebookAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddFacebookAuthentication
            webApplicationBuilder.Services.AddFacebookAuthentication(authenticationSetting);
        }
    }
}