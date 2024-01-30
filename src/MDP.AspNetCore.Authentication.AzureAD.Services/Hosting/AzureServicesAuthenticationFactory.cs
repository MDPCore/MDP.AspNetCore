using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public class AzureServicesAuthenticationFactory : ServiceFactory<WebApplicationBuilder, AzureServicesAuthenticationSetting>
    {
        // Constructors
        public AzureServicesAuthenticationFactory() : base("Authentication", "AzureAD.Services") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, AzureServicesAuthenticationSetting authenticationSetting)
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
