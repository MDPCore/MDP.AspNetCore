﻿using Microsoft.AspNetCore.Authentication;
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

namespace MDP.AspNetCore.Authentication.Apple
{
    public static class AppleExtensions
    {
        // Methods
        public static AuthenticationBuilder AddApple(this AuthenticationBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));

            #endregion

            // AddApple
            return builder.AddApple(_ => { });
        }

        public static AuthenticationBuilder AddApple(this AuthenticationBuilder builder, Action<AppleOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddApple
            return builder.AddApple(AppleDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddApple(this AuthenticationBuilder builder, string authenticationScheme, Action<AppleOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new ArgumentNullException($"{nameof(authenticationScheme)}=null");
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddOAuth
            return builder.AddOAuth<AppleOptions, AppleHandler>(authenticationScheme, null, configureOptions);
        }
    }
}
