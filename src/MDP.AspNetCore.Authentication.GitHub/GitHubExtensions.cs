using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.GitHub
{
    public static class GitHubExtensions
    {
        // Methods
        public static AuthenticationBuilder AddGitHub(this AuthenticationBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));

            #endregion

            // AddGitHub
            return builder.AddGitHub(_ => { });
        }

        public static AuthenticationBuilder AddGitHub(this AuthenticationBuilder builder, Action<GitHubOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddGitHub
            return builder.AddGitHub(GitHubDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddGitHub(this AuthenticationBuilder builder, string authenticationScheme, Action<GitHubOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new ArgumentNullException($"{nameof(authenticationScheme)}=null");
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddOAuth
            return builder.AddOAuth<GitHubOptions, GitHubHandler>(authenticationScheme, null, configureOptions);
        }
    }
}
