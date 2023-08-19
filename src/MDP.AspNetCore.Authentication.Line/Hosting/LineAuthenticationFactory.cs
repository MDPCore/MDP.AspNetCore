using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.Line
{
    [MDP.Registration.Factory<WebApplicationBuilder, LineAuthenticationSetting>("Authentication", "Line")]
    public class LineAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, LineAuthenticationSetting authenticationSetting)
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
