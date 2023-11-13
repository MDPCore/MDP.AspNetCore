using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    [MDP.Registration.Factory<WebApplicationBuilder, AzureServicesAuthenticationSetting>("Authentication", "AzureAD.Services")]
    public class AzureServicesAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, AzureServicesAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddAzureServicesAuthentication
            webApplicationBuilder.Services.AddAzureServicesAuthentication(authenticationSetting);
        }
    }
}
