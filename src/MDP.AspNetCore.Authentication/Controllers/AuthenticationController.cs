using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.Xml;

namespace MDP.AspNetCore.Authentication
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthenticationController : Controller
    {
        // Fields
        private readonly AuthenticationControllerSetting _authenticationSetting = null;

        private readonly AuthenticationProvider _authenticationProvider = null;


        // Constructors
        public AuthenticationController(AuthenticationControllerSetting authenticationSetting, AuthenticationProvider authenticationProvider = null)
        {
            #region Contracts

            if (authenticationSetting == null) throw new ArgumentNullException($"{nameof(authenticationSetting)}");

            #endregion

            // Default
            _authenticationSetting = authenticationSetting;
            _authenticationProvider = authenticationProvider ?? new AuthenticationProvider();
        }


        // Methods
        [AllowAnonymous]
        [Route("/.auth/login/{scheme}", Name = "/.auth/login")]
        public Task<ActionResult> Login(string scheme, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException(nameof(scheme));

            #endregion

            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // Login
            return this.LoginAsync(scheme, returnUrl);
        }

        [AllowAnonymous]
        [Route("/.auth/logout", Name = "/.auth/logout")]
        public Task<ActionResult> Logout(string returnUrl = null)
        {
            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // Login
            return this.LogoutAsync(returnUrl);
        }

        [AllowAnonymous]
        [Route("/.auth/signin", Name = "/.auth/signin")]
        public async Task<ActionResult> SignIn(string returnUrl = null)
        {
            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // RemoteIdentity
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity == null) throw new InvalidOperationException($"{nameof(remoteIdentity)}=null");

            // LocalIdentity
            var localIdentity = await this.LocalAuthenticateAsync();
            if (localIdentity != null && localIdentity.AuthenticationType == remoteIdentity.AuthenticationType) return this.Redirect(returnUrl);

            // Login
            if (localIdentity != null)
            {
                // RemoteLink
                _authenticationProvider.RemoteLink(remoteIdentity, localIdentity);

                // RemoteLogin
                localIdentity = _authenticationProvider.RemoteLogin(remoteIdentity);
                if (localIdentity == null) throw new InvalidOperationException("Identity link failed.");
            }
            if (localIdentity == null) localIdentity = _authenticationProvider.RemoteLogin(remoteIdentity);

            // SignIn
            if (localIdentity != null)
            {
                // LocalSignIn
                await this.HttpContext.RemoteSignOutAsync();
                await this.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

                // Redirect
                return this.Redirect(returnUrl);
            }

            // Register
            if (localIdentity == null && string.IsNullOrEmpty(_authenticationSetting.RegisterPath) == false)
            {
                // RemoteSignIn
                await this.HttpContext.RemoteSignInAsync(new ClaimsPrincipal(remoteIdentity));
                await this.HttpContext.LocalSignOutAsync();

                // Redirect
                return this.Redirect(_authenticationSetting.RegisterPath);
            }

            // Forbid
            {
                // SignOut
                await this.HttpContext.RemoteSignOutAsync();
                await this.HttpContext.LocalSignOutAsync();

                // Forbid
                return this.Forbid();
            }
        }
    }
}