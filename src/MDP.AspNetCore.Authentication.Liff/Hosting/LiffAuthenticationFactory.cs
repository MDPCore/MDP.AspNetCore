using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Liff
{
    public class LiffAuthenticationFactory : Factory<WebApplicationBuilder, LiffAuthenticationSetting>
    {
        // Constructors
        public LiffAuthenticationFactory() : base("Authentication", "Liff") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, LiffAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddLiffAuthentication
            webApplicationBuilder.Services.AddLiffAuthentication(authenticationSetting);
        }
    }
}
