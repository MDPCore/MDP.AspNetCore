using MDP.AspNetCore.Authentication.OAuthSSO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public static class ControllerExtensions
    {
        // Methods
        public static async Task<ActionResult> RefreshAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(controller);
            ArgumentNullException.ThrowIfNullOrEmpty(scheme);

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
            if (scheme.Equals(OAuthSSODefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase)==false) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignOutAsync();

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> LogoutAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(controller);
            ArgumentNullException.ThrowIfNullOrEmpty(scheme);

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
            if (scheme.Equals(OAuthSSODefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == false) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");

            // AuthOptionsMonitor
            var authOptionsMonitor = controller.HttpContext.RequestServices.GetService<IOptionsMonitor<OAuthSSOOptions>>();
            if (authOptionsMonitor == null) throw new InvalidOperationException($"{nameof(authOptionsMonitor)}=null");

            // AuthOptions
            var authOptions = authOptionsMonitor.Get(OAuthSSODefaults.AuthenticationScheme);
            if (authOptions == null) throw new InvalidOperationException($"{nameof(authOptions)}=null");

            // State
            var state = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(state) == true) throw new InvalidOperationException($"{nameof(state)}=null");

            // RedirectUri
            var redirectUri = $"{controller.HttpContext.Request.Scheme}://{controller.HttpContext.Request.Host}{returnUrl}";
            if (string.IsNullOrEmpty(redirectUri) == true) throw new InvalidOperationException($"{nameof(redirectUri)}=null");

            // LogoutUrl
            var logoutUrl = $"{authOptions.LogoutEndpoint}?client_id={authOptions.ClientId}&redirect_uri={redirectUri}&state={state}";
            if (string.IsNullOrEmpty(logoutUrl) == true) throw new InvalidOperationException($"{nameof(logoutUrl)}=null");

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignOutAsync();

            // Redirect
            return controller.Redirect(logoutUrl);
        }
    }
}