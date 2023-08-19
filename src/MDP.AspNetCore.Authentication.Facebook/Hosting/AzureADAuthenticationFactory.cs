using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.Facebook
{
    [MDP.Registration.Factory<WebApplicationBuilder, FacebookAuthenticationSetting>("Authentication", "Facebook")]
    public class FacebookAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, FacebookAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddFacebookAuthentication
            webApplicationBuilder.Services.AddFacebookAuthentication(authenticationSetting);
        }
    }
}
