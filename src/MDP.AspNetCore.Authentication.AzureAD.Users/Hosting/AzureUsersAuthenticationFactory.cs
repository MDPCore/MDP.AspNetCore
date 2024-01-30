using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    public class AzureUsersAuthenticationFactory : ServiceFactory<WebApplicationBuilder, AzureUsersAuthenticationSetting>
    {
        // Constructors
        public AzureUsersAuthenticationFactory() : base("Authentication", "AzureAD.Users") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder webApplicationBuilder, AzureUsersAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddAzureUsersAuthentication
            webApplicationBuilder.Services.AddAzureUsersAuthentication(authenticationSetting);
        }
    }
}