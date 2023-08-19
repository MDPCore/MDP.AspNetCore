using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.Jwt
{
    [MDP.Registration.Factory<WebApplicationBuilder, SecurityTokenAuthenticationSetting>("Authentication", "Jwt")]
    public class SecurityTokenAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, SecurityTokenAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddSecurityTokenAuthentication
            webApplicationBuilder.Services.AddSecurityTokenAuthentication(authenticationSetting);
        }
    }
}
