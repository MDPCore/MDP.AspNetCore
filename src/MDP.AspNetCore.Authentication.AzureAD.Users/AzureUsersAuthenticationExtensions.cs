using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    public static class AzureUsersAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddAzureUsersAuthentication(this IServiceCollection services, AzureUsersAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
         
            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new AzureUsersAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.TenantId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.TenantId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // AzureUsers
            authenticationBuilder.AddAzureUsers(options =>
            {
                // Options
                options.TenantId = authenticationSetting.TenantId; // {TenantId},organizations,common,consumers
                options.ClientId = authenticationSetting.ClientId;
                options.ClientSecret = authenticationSetting.ClientSecret;
                options.CallbackPath = new PathString("/.auth/login/aad/users/callback");

                // SignIn
                options.ConfigureSignIn();
            });

            // Return
            return authenticationBuilder;
        }
    }
}
