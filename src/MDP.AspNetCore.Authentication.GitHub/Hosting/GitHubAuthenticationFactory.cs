using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.GitHub
{
    public class GitHubAuthenticationFactory : Factory<WebApplicationBuilder, GitHubAuthenticationSetting>
    {
        // Constructors
        public GitHubAuthenticationFactory() : base("Authentication", "GitHub") { }


        // Methods
        public override List<ServiceRegistration> ConfigureService(WebApplicationBuilder webApplicationBuilder, GitHubAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (webApplicationBuilder == null) throw new ArgumentException($"{nameof(webApplicationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AddGitHubAuthentication
            webApplicationBuilder.Services.AddGitHubAuthentication(authenticationSetting);

            // Return
            return null;
        }
    }
}