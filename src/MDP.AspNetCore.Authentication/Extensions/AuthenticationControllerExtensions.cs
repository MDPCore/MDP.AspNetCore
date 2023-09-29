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
    public static partial class AuthenticationControllerExtensions
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

    public static partial class AuthenticationControllerExtensions
    {
        // Methods
        internal static async Task<ActionResult> SignInAsync(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");

            // AuthenticationProvider
            var authenticationProvider = controller.HttpContext.RequestServices.GetService<AuthenticationProvider>();

            // AuthenticationControllerSetting
            var authenticationControllerSetting = controller.HttpContext.RequestServices.GetService<AuthenticationControllerSetting>();
            if (authenticationControllerSetting == null) throw new InvalidOperationException($"{nameof(authenticationControllerSetting)}=null");

            // RemoteIdentity
            var remoteIdentity = await controller.RemoteAuthenticateAsync();
            if (remoteIdentity == null) throw new InvalidOperationException($"{nameof(remoteIdentity)}=null");

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity != null && authenticationProvider == null) return controller.Redirect(returnUrl);
            if (localIdentity != null && localIdentity.AuthenticationType == remoteIdentity.AuthenticationType) return controller.Redirect(returnUrl);

            // Link
            if (localIdentity != null && authenticationProvider != null)
            {
                // Link
                authenticationProvider.RemoteLink(remoteIdentity, localIdentity);

                // Exchange
                localIdentity = authenticationProvider.RemoteExchange(remoteIdentity);
                if (localIdentity == null) throw new InvalidOperationException("Identity link failed.");
            }

            // Login
            if (localIdentity == null && authenticationProvider == null) localIdentity = remoteIdentity;
            if (localIdentity == null && authenticationProvider != null) localIdentity = authenticationProvider.RemoteExchange(remoteIdentity);
            if (localIdentity != null)
            {
                // Sign
                await controller.HttpContext.RemoteSignOutAsync();
                await controller.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

                // Redirect
                return controller.Redirect(returnUrl);
            }

            // Register
            if (localIdentity == null && string.IsNullOrEmpty(authenticationControllerSetting.RegisterPath) == false)
            {
                // Sign
                await controller.HttpContext.RemoteSignInAsync(new ClaimsPrincipal(remoteIdentity));
                await controller.HttpContext.LocalSignOutAsync();

                // Register
                return controller.Redirect(authenticationControllerSetting.RegisterPath);
            }

            // Forbid
            {
                // Sign
                await controller.HttpContext.RemoteSignOutAsync();
                await controller.HttpContext.LocalSignOutAsync();

                // Forbid
                return controller.Forbid();
            }
        }
    }
}