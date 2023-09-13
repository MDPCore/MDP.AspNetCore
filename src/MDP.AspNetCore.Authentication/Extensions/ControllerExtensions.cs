using Microsoft.AspNetCore.Authentication;
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
        public static async Task<ActionResult> LoginAsync(this Controller controller, ClaimsIdentity remoteIdentity, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");
            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");
            if (remoteIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(remoteIdentity.IsAuthenticated)}=false");

            // LocalIdentity
            var localIdentity = remoteIdentity;
            var localAuthenticationProvider = controller.HttpContext.RequestServices.GetService<LocalAuthenticationProvider>();
            if (localAuthenticationProvider != null)
            {
                // Login
                localIdentity = localAuthenticationProvider.Login(remoteIdentity);
                if (localIdentity == null)
                {
                    if (string.IsNullOrEmpty(localAuthenticationProvider.RegisterPath) == true)
                    {
                        // Forbid
                        return controller.Forbid();
                    }
                    else
                    {
                        // Register
                        return controller.Redirect(localAuthenticationProvider.RegisterPath);
                    }
                }
            }

            // SignIn
            await controller.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));
            await controller.HttpContext.RemoteSignOutAsync();

            // Redirect
            return controller.Redirect(returnUrl);
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

            // Authenticate
            var authenticateResult = await controller.HttpContext.LocalAuthenticateAsync();
            if (authenticateResult.Principal?.Identity?.IsAuthenticated == true) return controller.Redirect(returnUrl);

            // SignOut
            await controller.HttpContext.LocalSignOutAsync();
            await controller.HttpContext.RemoteSignOutAsync();

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> LogoutAsync(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");

            // SignOut
            await controller.HttpContext.LocalSignOutAsync();
            await controller.HttpContext.RemoteSignOutAsync();

            // Redirect
            return controller.Redirect(returnUrl);
        }
    }
}