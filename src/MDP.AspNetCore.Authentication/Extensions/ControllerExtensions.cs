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

            // Authenticate
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity == null) throw new InvalidOperationException("Identity authenticate failed.");

            // Login
            var remoteIdentity = await controller.RemoteAuthenticateAsync();
            if (localIdentity == null && remoteIdentity != null && remoteIdentity.AuthenticationType == scheme)
            {
                return await controller.LoginAsync(remoteIdentity, returnUrl);
            }

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

            // Authenticate
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity != null) return controller.Redirect(returnUrl);

            // Login
            var remoteIdentity = await controller.RemoteAuthenticateAsync();
            if (localIdentity == null && remoteIdentity != null && remoteIdentity.AuthenticationType == scheme)
            {
                return await controller.LoginAsync(remoteIdentity, returnUrl);
            }

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> LoginAsync(this Controller controller, ClaimsIdentity remoteIdentity, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");
            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");
            if (remoteIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(remoteIdentity.IsAuthenticated)}=false");

            // AuthenticationProvider
            var authenticationProvider = controller.HttpContext.RequestServices.GetService<AuthenticationProvider>();

            // AuthenticationSetting
            var authenticationSetting = controller.HttpContext.RequestServices.GetService<DefaultAuthenticationSetting>();
            if (authenticationSetting == null) throw new InvalidOperationException($"{nameof(authenticationSetting)}=null");

            // Authenticate
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity != null && authenticationProvider == null) return controller.Redirect(returnUrl);
            if (localIdentity != null && localIdentity.AuthenticationType == remoteIdentity.AuthenticationType) return controller.Redirect(returnUrl);

            // Link
            if (localIdentity != null && authenticationProvider != null)
            {
                // Link
                authenticationProvider.Link(localIdentity, remoteIdentity);

                // Login
                localIdentity = authenticationProvider.Login(remoteIdentity);
                if(localIdentity==null) throw new InvalidOperationException("Identity link failed.");
            }

            // Login
            if (localIdentity == null && authenticationProvider == null) localIdentity = remoteIdentity;
            if (localIdentity == null && authenticationProvider != null) localIdentity = authenticationProvider.Login(remoteIdentity);
            if (localIdentity != null)
            {
                // Sign
                await controller.HttpContext.RemoteSignOutAsync();
                await controller.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

                // Redirect
                return controller.Redirect(returnUrl);
            }

            // Register
            if (localIdentity == null && string.IsNullOrEmpty(authenticationSetting.RegisterPath) == false)
            {
                // Sign
                await controller.HttpContext.RemoteSignInAsync(new ClaimsPrincipal(remoteIdentity));
                await controller.HttpContext.LocalSignOutAsync();

                // Register
                return controller.Redirect(authenticationSetting.RegisterPath);
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

        public static async Task<ActionResult> LogoutAsync(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");

            #endregion

            // Require
            returnUrl = returnUrl ?? controller.Url.Content("~/");

            // SignOut
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