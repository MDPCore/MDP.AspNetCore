using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.Google
{
    [MDP.Registration.Factory<WebApplicationBuilder, GoogleAuthenticationSetting>("Authentication", "Google")]
    public class GoogleAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, GoogleAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddGoogleAuthentication
            webApplicationBuilder.Services.AddGoogleAuthentication(authenticationSetting);
        }
    }
}
