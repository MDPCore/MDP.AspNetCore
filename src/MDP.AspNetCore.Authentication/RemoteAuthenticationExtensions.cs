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
    public static partial class RemoteAuthenticationExtensions
    {
        // Methods
        internal static AuthenticationBuilder AddRemote(this AuthenticationBuilder builder, Action<CookieAuthenticationOptions> configureOptions = null)
        {
            #region Contracts

            if (builder == null) throw new ArgumentNullException($"{nameof(builder)}=null");

            #endregion

            // AuthenticationOptions
            if (configureOptions == null)
            {
                builder.AddCookie(RemoteAuthenticationDefaults.AuthenticationScheme);
            }
            else
            {
                builder.AddCookie(RemoteAuthenticationDefaults.AuthenticationScheme, null, configureOptions);
            }

            // Return
            return builder;
        }
    }

    public static partial class RemoteAuthenticationExtensions
    {
        // Methods
        internal static async Task<ClaimsIdentity> RemoteAuthenticateAsync(this HttpContext httpContext)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            #endregion

            // AuthenticateAsync
            var authenticateResult = await httpContext.AuthenticateAsync(RemoteAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult== null) throw new InvalidOperationException($"{nameof(authenticateResult)}=null");
            if (authenticateResult.Succeeded == false) return null;

            // Identity
            var identity = authenticateResult?.Principal?.Identity;
            if (identity == null) return null;
            if (identity.IsAuthenticated == false) return null;

            // Return
            return identity as ClaimsIdentity;
        }

        internal static Task RemoteSignInAsync(this HttpContext httpContext, ClaimsPrincipal principal)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentNullException($"{nameof(httpContext)}=null");
            if (principal == null) throw new ArgumentNullException($"{nameof(principal)}=null");

            #endregion

            // SignInAsync
            return httpContext.SignInAsync(RemoteAuthenticationDefaults.AuthenticationScheme, principal);
        }

        public static Task RemoteSignOutAsync(this HttpContext httpContext)
        {
            #region Contracts

            if (httpContext == null) throw new ArgumentNullException($"{nameof(httpContext)}=null");

            #endregion

            // SignOutAsync
            return httpContext.SignOutAsync(RemoteAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public static partial class RemoteAuthenticationExtensions
    {
        // Methods
        public static void ConfigureSignIn(this RemoteAuthenticationOptions remoteAuthenticationOptions, string signInPath = "/.auth/signin")
        {
            #region Contracts

            if (string.IsNullOrEmpty(signInPath) == true) throw new ArgumentNullException(nameof(signInPath));

            #endregion

            // SignInScheme
            remoteAuthenticationOptions.SignInScheme = RemoteAuthenticationDefaults.AuthenticationScheme;

            // OnTicketReceived
            remoteAuthenticationOptions.Events.OnTicketReceived = context =>
            {
                // ReturnUrl
                var returnUrl = new PathString(signInPath).Add(QueryString.Create(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "returnUrl", context.ReturnUri! }
                }!));
                if (string.IsNullOrEmpty(returnUrl) == true) throw new InvalidOperationException($"{nameof(returnUrl)}=null");

                // Setting
                context.ReturnUri = returnUrl;

                // Return
                return Task.CompletedTask;
            };
        }
    }
}
