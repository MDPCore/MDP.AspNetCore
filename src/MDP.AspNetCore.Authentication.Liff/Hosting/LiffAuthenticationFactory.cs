using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.Liff
{
    [MDP.Registration.Factory<WebApplicationBuilder, LiffAuthenticationSetting>("Authentication", "Liff")]
    public class LiffAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, LiffAuthenticationSetting authenticationSetting)
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
