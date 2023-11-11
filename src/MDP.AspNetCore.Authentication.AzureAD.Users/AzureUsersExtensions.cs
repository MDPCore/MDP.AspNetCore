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

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    public static class AzureUsersExtensions
    {
        // Methods
        public static AuthenticationBuilder AddAzureUsers(this AuthenticationBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException(nameof(builder));

            #endregion

            // AddAzureUsers
            return builder.AddAzureUsers(_ => { });
        }

        public static AuthenticationBuilder AddAzureUsers(this AuthenticationBuilder builder, Action<AzureUsersOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException(nameof(builder));
            if (configureOptions == null) throw new ArgumentException(nameof(configureOptions));

            #endregion

            // AddAzureUsers
            return builder.AddAzureUsers(AzureUsersDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddAzureUsers(this AuthenticationBuilder builder, string authenticationScheme, Action<AzureUsersOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException(nameof(builder));
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new ArgumentException($"{nameof(authenticationScheme)}=null");
            if (configureOptions == null) throw new ArgumentException(nameof(configureOptions));

            #endregion

            // AddOAuth
            return builder.AddOAuth<AzureUsersOptions, AzureUsersHandler>(authenticationScheme, null, configureOptions);
        }
    }
}
