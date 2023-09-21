using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.Microsoft
{
    [MDP.Registration.Factory<WebApplicationBuilder, MicrosoftAuthenticationSetting>("Authentication", "Microsoft")]
    public class MicrosoftAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, MicrosoftAuthenticationSetting authenticationSetting)
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
