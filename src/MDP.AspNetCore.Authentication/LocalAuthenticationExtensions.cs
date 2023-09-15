using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    internal static partial class LocalAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddLocal(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configureOptions = null)
        {
            #region Contracts

            if (builder == null) throw new ArgumentException($"{nameof(builder)}=null");

            #endregion

            // AuthenticationOptions
            if (configureOptions == null)
            {
                builder.AddCookie(LocalAuthenticationDefaults.AuthenticationScheme);
            }
            else
            {
                builder.AddCookie(LocalAuthenticationDefaults.AuthenticationScheme, null, configureOptions);
            }

            // Return
            return builder;
        }
    }

    internal static partial class LocalAuthenticationExtensions
    {
        // Methods
        public static async Task<ClaimsIdentity> LocalAuthenticateAsync(this HttpContext httpContext)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentException(nameof(httpContext));

            #endregion

            // AuthenticateAsync
            var authenticateResult = await httpContext.AuthenticateAsync();
            if (authenticateResult == null) throw new InvalidOperationException($"{nameof(authenticateResult)}=null");
            if (authenticateResult.Succeeded == false) return null;

            // Identity
            var identity = authenticateResult?.Principal?.Identity;
            if (identity == null) return null;
            if (identity.IsAuthenticated == false) return null;

            // Return
            return identity as ClaimsIdentity;
        }

        public static Task LocalSignInAsync(this HttpContext httpContext, ClaimsPrincipal principal)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentException($"{nameof(httpContext)}=null");
            if (principal == null) throw new ArgumentException($"{nameof(principal)}=null");

            #endregion

            // SignInAsync
            return httpContext.SignInAsync(principal);
        }

        public static Task LocalSignOutAsync(this HttpContext httpContext)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentException($"{nameof(httpContext)}=null");

            #endregion

            // SignOutAsync
            return httpContext.SignOutAsync();
        }
    }
}
