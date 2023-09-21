using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.Microsoft
{
    public static class MicrosoftAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddMicrosoftAuthentication(this IServiceCollection services, MicrosoftAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
         
            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new MicrosoftAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.ClientId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // Microsoft
            authenticationBuilder.AddMicrosoft(options =>
            {
                // Options
                options.TenantId = "consumers";
                options.ClientId = authenticationSetting.ClientId;
                options.ClientSecret = authenticationSetting.ClientSecret;
                options.CallbackPath = new PathString("/.auth/login/microsoft/callback");

                // SignIn
                options.ConfigureSignIn();
            });

            // Return
            return authenticationBuilder;
        }
    }
}
