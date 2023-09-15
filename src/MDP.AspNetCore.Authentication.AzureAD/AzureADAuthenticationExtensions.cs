using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD
{
    public static class AzureADAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddAzureADAuthentication(this IServiceCollection services, AzureADAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
         
            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new AzureADAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.TenantId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.TenantId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // AzureAD
            authenticationBuilder.AddAzureAD(options =>
            {
                // Options
                options.TenantId = authenticationSetting.TenantId;
                options.ClientId = authenticationSetting.ClientId;
                options.ClientSecret = authenticationSetting.ClientSecret;
                options.CallbackPath = new PathString("/.auth/login/azuread/callback");

                // SignIn
                options.ConfigureSignIn();
            });

            // Return
            return authenticationBuilder;
        }
    }
}
