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
using MDP.AspNetCore.Authentication.Line;

namespace MDP.AspNetCore.Authentication.Liff
{
    public static class LiffExtensions
    {
        // Methods
        public static AuthenticationBuilder AddLiff(this AuthenticationBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));

            #endregion

            // AddLiff
            return builder.AddLiff(_ => { });
        }

        public static AuthenticationBuilder AddLiff(this AuthenticationBuilder builder, Action<LiffOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddLiff
            return builder.AddLiff(LiffDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddLiff(this AuthenticationBuilder builder, string authenticationScheme, Action<LiffOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new ArgumentNullException($"{nameof(authenticationScheme)}=null");
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddOAuth
            return builder.AddOAuth<LiffOptions, LiffHandler>(authenticationScheme, null, configureOptions);
        }
    }
}
