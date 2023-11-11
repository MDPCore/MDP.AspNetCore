using Microsoft.AspNetCore.Builder;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    [MDP.Registration.Factory<WebApplicationBuilder, AzureUsersAuthenticationSetting>("Authentication", "AzureAD.Users")]
    public class AzureUsersAuthenticationFactory
    {
        // Methods
        public void ConfigureService(WebApplicationBuilder webApplicationBuilder, AzureUsersAuthenticationSetting authenticationSetting)
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
