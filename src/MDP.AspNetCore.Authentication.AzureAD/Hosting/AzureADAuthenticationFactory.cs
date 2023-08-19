using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD
{
    [MDP.Registration.Factory<WebApplicationBuilder, AzureADAuthenticationSetting>("Authentication", "AzureAD")]
    public class AzureADAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, AzureADAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddAzureADAuthentication
            webApplicationBuilder.Services.AddAzureADAuthentication(authenticationSetting);
        }
    }
}
