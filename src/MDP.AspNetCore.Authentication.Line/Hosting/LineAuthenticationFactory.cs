using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineAuthenticationFactory : ServiceFactory<WebApplicationBuilder, LineAuthenticationSetting>
    {
        // Constructors
        public LineAuthenticationFactory() : base("Authentication", "Line") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder authenticationBuilder, LineAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddLineAuthentication
            authenticationBuilder.Services.AddLineAuthentication(authenticationSetting);
        }
    }
}

