using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication
{
    [MDP.Registration.Factory<WebApplicationBuilder, DefaultAuthenticationSetting>("Authentication")]
    public class DefaultAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, DefaultAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddDefaultAuthentication
            webApplicationBuilder.Services.AddDefaultAuthentication(authenticationSetting);
        }
    }
}
