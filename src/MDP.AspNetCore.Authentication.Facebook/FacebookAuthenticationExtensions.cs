using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.Facebook
{
    public static class FacebookAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddFacebookAuthentication(this IServiceCollection services, FacebookAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
         
            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new FacebookAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.ClientId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // Facebook
            authenticationBuilder.AddFacebook(options =>
            {
                // Options
                options.ClientId = authenticationSetting.ClientId;
                options.ClientSecret = authenticationSetting.ClientSecret;
                options.CallbackPath = new PathString("/.auth/login/facebook/callback");

                // SignIn
                options.ConfigureSignIn();
            });

            // Return
            return authenticationBuilder;
        }
    }
}
