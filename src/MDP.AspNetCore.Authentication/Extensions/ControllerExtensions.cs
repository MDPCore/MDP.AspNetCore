using Microsoft.AspNetCore.Authentication;
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
    public static partial class ControllerExtensions
    {
        // Methods
        public static async Task<ActionResult> LinkAsync(this Controller controller, string scheme, string returnUrl = null)
        {
            #region Contracts

            if (controller == null) throw new ArgumentException($"{nameof(controller)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");

            #endregion

            // Require
            returnUrl = controller.NormalizeReturnUrl(returnUrl);            
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
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
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
            returnUrl = controller.NormalizeReturnUrl(returnUrl);
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
            returnUrl = controller.NormalizeReturnUrl(returnUrl);

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

    public static partial class ControllerExtensions
    {
        // Methods
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