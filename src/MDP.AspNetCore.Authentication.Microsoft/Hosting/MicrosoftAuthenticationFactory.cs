using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Microsoft
{
    public class MicrosoftAuthenticationFactory : ServiceFactory<WebApplicationBuilder, MicrosoftAuthenticationSetting>
    {
        // Constructors
        public MicrosoftAuthenticationFactory() : base("Authentication", "Microsoft") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, MicrosoftAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddMicrosoftAuthentication
            webApplicationBuilder.Services.AddMicrosoftAuthentication(authenticationSetting);
        }
    }
}