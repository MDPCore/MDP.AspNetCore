﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public static class ControllerExtensions
    {
        // Methods
        public static async Task<ActionResult> LoginAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentNullException($"{nameof(scheme)}=null");

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
            if (scheme.Equals(PolicyAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(LocalAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(RemoteAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity != null && localIdentity.IsAuthenticated == true)
            {
                // Redirect
                controller.Resolve(returnUrl);
            }

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignOutAsync();

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> LinkAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentNullException($"{nameof(scheme)}=null");

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
            if (scheme.Equals(PolicyAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(LocalAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
            if (scheme.Equals(RemoteAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity == null) throw new InvalidOperationException($"{nameof(localIdentity)}=null");
            if (localIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(localIdentity)}.IsAuthenticated=false");

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();

            // Challenge
            return controller.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
        }

        public static async Task<ActionResult> RefreshAsync(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);

            // AuthenticationProvider
            var authenticationProvider = controller.HttpContext.RequestServices.GetService<AuthenticationProvider>();
            if (authenticationProvider == null) authenticationProvider = new AuthenticationProvider();

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity == null) throw new InvalidOperationException($"{nameof(localIdentity)}=null");
            if (localIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(localIdentity)}.IsAuthenticated=false");

            // Login
            if (localIdentity != null) localIdentity = authenticationProvider.LocalLogin(localIdentity);
            if (localIdentity == null) throw new InvalidOperationException($"{nameof(localIdentity)}=null");
            if (localIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(localIdentity)}.IsAuthenticated=false");

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

            // Redirect
            return controller.Redirect(returnUrl);
        }

        public static async Task<ActionResult> LogoutAsync(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);

            // Sign
            await controller.HttpContext.RemoteSignOutAsync();
            await controller.HttpContext.LocalSignOutAsync();

            // Redirect
            return controller.Redirect(returnUrl);
        }
                

        public static async Task<ActionResult> SignInAsync(this Controller controller, ClaimsIdentity remoteIdentity, string returnUrl = null)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(controller);
            ArgumentNullException.ThrowIfNull(remoteIdentity);

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);

            // AuthenticationProvider
            var authenticationProvider = controller.HttpContext.RequestServices.GetService<AuthenticationProvider>();
            if (authenticationProvider == null) authenticationProvider = new AuthenticationProvider();

            // AuthenticationSetting
            var authenticationSetting = controller.HttpContext.RequestServices.GetService<AuthenticationSetting>();
            if (authenticationSetting == null) authenticationSetting = new AuthenticationSetting();

            // RemoteIdentity
            if (remoteIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(remoteIdentity.IsAuthenticated)}=false");

            // LocalIdentity
            var localIdentity = await controller.LocalAuthenticateAsync();
            if (localIdentity != null && localIdentity.IsAuthenticated == false) localIdentity = null;

            // Link
            if (localIdentity != null)
            {
                // RemoteLink
                authenticationProvider.RemoteLink(remoteIdentity, localIdentity);
            }

            // Login
            if (localIdentity == null)
            {
                // RemoteLogin
                localIdentity = authenticationProvider.RemoteLogin(remoteIdentity);
                if (localIdentity != null && localIdentity.IsAuthenticated == false) localIdentity = null;
            }

            // SignIn
            if (localIdentity != null)
            {
                // Sign
                await controller.HttpContext.RemoteSignOutAsync();
                await controller.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

                // Redirect
                return controller.Resolve(returnUrl);
            }

            // Register
            if (localIdentity == null && string.IsNullOrEmpty(authenticationSetting.RegisterPath) == false)
            {
                // Sign
                await controller.HttpContext.RemoteSignInAsync(new ClaimsPrincipal(remoteIdentity));
                await controller.HttpContext.LocalSignOutAsync();

                // Redirect
                return controller.Register(returnUrl);
            }

            // Forbid
            {
                // Sign
                await controller.HttpContext.RemoteSignOutAsync();
                await controller.HttpContext.LocalSignOutAsync();

                // Redirect
                return controller.Forbid();
            }
        }

        private static ActionResult Resolve(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");

            #endregion

            // AuthenticationSetting
            var authenticationSetting = controller.HttpContext.RequestServices.GetService<AuthenticationSetting>();
            if (authenticationSetting == null) authenticationSetting = new AuthenticationSetting();

            // RedirectUrl
            var redirectUrl = string.Empty;
            if (string.IsNullOrEmpty(authenticationSetting.ResolvePath)== true)
            {
                redirectUrl = returnUrl;
            }
            else 
            {
                redirectUrl = $"{authenticationSetting.ResolvePath}?returnUrl={Uri.EscapeDataString(returnUrl)}";
            }

            // Redirect
            return controller.Redirect(redirectUrl);
        }

        private static ActionResult Register(this Controller controller, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");

            #endregion

            // AuthenticationSetting
            var authenticationSetting = controller.HttpContext.RequestServices.GetService<AuthenticationSetting>();
            if (authenticationSetting == null) authenticationSetting = new AuthenticationSetting();

            // RedirectUrl
            var redirectUrl = string.Empty;
            if (string.IsNullOrEmpty(authenticationSetting.RegisterPath) == true)
            {
                redirectUrl = returnUrl;
            }
            else
            {
                redirectUrl = $"{authenticationSetting.RegisterPath}?returnUrl={Uri.EscapeDataString(returnUrl)}";
            }

            // Redirect
            return controller.Redirect(redirectUrl);
        }


        internal static Task<ClaimsIdentity> RemoteAuthenticateAsync(this Controller controller)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");

            #endregion

            // Return
            return controller.HttpContext.RemoteAuthenticateAsync();
        }

        internal static Task<ClaimsIdentity> LocalAuthenticateAsync(this Controller controller)
        {
            #region Contracts

            if (controller == null) throw new ArgumentNullException($"{nameof(controller)}=null");

            #endregion

            // Return
            return controller.HttpContext.LocalAuthenticateAsync();
        }


        public static string NormalizeReturnUrl(this Controller controller, string returnUrl = null)
        {
            // Require
            if (returnUrl == "/") return "/";
            if (string.IsNullOrEmpty(returnUrl) == true) return "/";

            // Absolute
            if (Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute) == true)
            {
                try
                {
                    // Uri
                    var returnUri = new Uri(returnUrl);

                    // RelativePath
                    var relativePath = returnUri.AbsolutePath;
                    if (relativePath.StartsWith("/") == false)
                    {
                        relativePath = "/" + relativePath;
                    }

                    // QueryString
                    if (string.IsNullOrEmpty(returnUri.Query) == false)
                    {
                        relativePath += returnUri.Query;
                    }

                    // Return
                    return relativePath;
                }
                catch (UriFormatException)
                {
                    // Return
                    return "/";
                }
            }

            // Relative
            {
                // RelativePath
                var relativePath = returnUrl;

                // StartsWith
                if (relativePath.StartsWith("/") == false)
                {
                    relativePath = "/" + relativePath;
                }

                // Return
                return relativePath;
            }
        }
    }
}