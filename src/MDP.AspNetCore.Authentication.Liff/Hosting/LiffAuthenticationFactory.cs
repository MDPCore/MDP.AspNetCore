using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Liff
{
    public class LiffAuthenticationFactory : ServiceFactory<WebApplicationBuilder, LiffAuthenticationSetting>
    {
        // Constructors
        public LiffAuthenticationFactory() : base("Authentication", "Liff") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder authenticationBuilder, LiffAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddLiffAuthentication
            authenticationBuilder.Services.AddLiffAuthentication(authenticationSetting);
        }
    }
}
