using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.GitHub
{
    public static class GitHubAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddGitHubAuthentication(this IServiceCollection services, GitHubAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
         
            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new GitHubAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.ClientId) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientSecret) == true) throw new InvalidOperationException($"{nameof(authenticationSetting.ClientSecret)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // GitHub
            authenticationBuilder.AddGitHub(options =>
            {
                // Options
                options.ClientId = authenticationSetting.ClientId;
                options.ClientSecret = authenticationSetting.ClientSecret;

                // SignIn
                options.SignInPath("/.auth/signin");
                options.SignInScheme = RemoteAuthenticationDefaults.AuthenticationScheme;
            });

            // Return
            return authenticationBuilder;
        }
    }
}
