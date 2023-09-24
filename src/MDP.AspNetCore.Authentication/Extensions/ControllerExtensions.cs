using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public static class ControllerExtensions
    {
        // Methods
        public static async Task<ActionResult> LinkAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");
            if (scheme.Equals(PolicyAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(LocalAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(RemoteAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity == null) throw new InvalidOperationException($"{nameof(localIdentity)}=null");

            // RemoteIdentity
            var remoteIdentity = await controller.RemoteAuthenticateAsync();
            if (remoteIdentity != null) await controller.HttpContext.RemoteSignOutAsync();

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> LoginAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");
            if (scheme.Equals(PolicyAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(LocalAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(RemoteAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity != null) throw new InvalidOperationException($"{nameof(localIdentity)}!=null");

            // RemoteIdentity
            var remoteIdentity = await controller.RemoteAuthenticateAsync();
            if (remoteIdentity != null) await controller.HttpContext.RemoteSignOutAsync();

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> LoginAsync(this Controller controller, ClaimsIdentity localIdentity, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");
            if (localIdentity == null) throw new ArgumentException($"{nameof(localIdentity)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");
            if (localIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(localIdentity.IsAuthenticated)}=false");

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

            // Redirect
            return controller.Redirect(returnUrl);
        }

        public static async Task<ActionResult> LogoutAsync(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignOutAsync();

            // Redirect
            return controller.Redirect(returnUrl);
        }


        public static Task<ClaimsIdentity> RemoteAuthenticateAsync(this Controller controller)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");

            #endregion

            // Return
            return controller.HttpContext.RemoteAuthenticateAsync();
        }

        public static Task<ClaimsIdentity> LocalAuthenticateAsync(this Controller controller)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");

            #endregion

            // Return
            return controller.HttpContext.LocalAuthenticateAsync();
        }
    }
}