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

namespace MDP.AspNetCore.Authentication.Line
{
    public static class LineExtensions
    {
        // Methods
        public static AuthenticationBuilder AddLine(this AuthenticationBuilder builder)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));

            #endregion

            // AddLine
            return builder.AddLine(_ => { });
        }

        public static AuthenticationBuilder AddLine(this AuthenticationBuilder builder, Action<LineOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddLine
            return builder.AddLine(LineDefaults.AuthenticationScheme, configureOptions);
        }

        public static AuthenticationBuilder AddLine(this AuthenticationBuilder builder, string authenticationScheme, Action<LineOptions> configureOptions)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new ArgumentNullException($"{nameof(authenticationScheme)}=null");
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            #endregion

            // AddOAuth
            return builder.AddOAuth<LineOptions, LineHandler>(authenticationScheme, null, configureOptions);
        }
    }
}
