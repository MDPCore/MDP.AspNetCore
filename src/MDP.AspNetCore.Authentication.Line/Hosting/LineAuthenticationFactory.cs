using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineAuthenticationFactory : Factory<WebApplicationBuilder, LineAuthenticationSetting>
    {
        // Constructors
        public LineAuthenticationFactory() : base("Authentication", "Line") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, LineAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddLineAuthentication
            webApplicationBuilder.Services.AddLineAuthentication(authenticationSetting);
        }
    }
}

