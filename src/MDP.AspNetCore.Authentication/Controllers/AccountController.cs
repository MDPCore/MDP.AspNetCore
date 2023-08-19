using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        // Fields
        private readonly AuthenticationProvider _authenticationProvider;


        // Constructors
        public AccountController(AuthenticationProvider authenticationProvider = null)
        {
            // Default
            _authenticationProvider = authenticationProvider ?? DefaultAuthenticationProvider.Current;
        }


        // Methods
        [AllowAnonymous]
        [Route("/.auth/login/{scheme}", Name = "/.auth/login")]
        public async Task<ActionResult> Login(string scheme = null, string returnUrl = null)
        {           
            // Require
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // RemoteAuth
            await this.HttpContext.RemoteSignOutAsync();

            // LocalAuth
            var localAuthenticateResult = await this.HttpContext.LocalAuthenticateAsync();
            if (localAuthenticateResult.Principal?.Identity?.IsAuthenticated == true) return this.Redirect(returnUrl);

            // Challenge
            if (string.IsNullOrEmpty(scheme) == false)
            {
                // Ignore
                if (scheme.Equals(PolicyAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
                if (scheme.Equals(LocalAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
                if (scheme.Equals(RemoteAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase) == true) throw new InvalidOperationException($"{nameof(scheme)}={scheme}");
                
                // Redirect
                return this.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl }, scheme);
            }

            // Return
            return this.Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        [AllowAnonymous]
        [Route("/.auth/logout", Name = "/.auth/logout")]
        public async Task<ActionResult> Logout(string returnUrl = null)
        {
            // Require
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // SignOut
            await this.HttpContext.LocalSignOutAsync();
            await this.HttpContext.RemoteSignOutAsync();

            // Return
            return this.Redirect(returnUrl);
        }

        [AllowAnonymous]
        [Route("/.auth/signin", Name = "/.auth/signin")]
        public async Task<ActionResult> SignIn(string returnUrl = null)
        {
            // Require
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // LocalAuth
            await this.HttpContext.LocalSignOutAsync();

            // RemoteAuth
            var remoteAuthenticateResult = await this.HttpContext.RemoteAuthenticateAsync();
            if (remoteAuthenticateResult.Principal?.Identity?.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(remoteAuthenticateResult)}.Principal?.Identity?.IsAuthenticated=false");

            // RemoteIdentity
            var remoteIdentity = remoteAuthenticateResult.Principal?.Identity as ClaimsIdentity;
            if (remoteIdentity == null) throw new InvalidOperationException($"{nameof(remoteIdentity)}=null");
            if (remoteIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(remoteIdentity.IsAuthenticated)}=false");

            // LocalIdentity
            var localIdentity = _authenticationProvider.SignIn(remoteIdentity);
            if (localIdentity == null)
            {
                if (string.IsNullOrEmpty(_authenticationProvider.RegisterPath) == false)
                {
                    // Register
                    return this.Redirect(_authenticationProvider.RegisterPath);                    
                }
                else
                {
                    // Forbid
                    return this.Forbid();
                }
            }

            // SignIn
            await this.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));
            await this.HttpContext.RemoteSignOutAsync();

            // Redirect
            return this.Redirect(returnUrl);
        }
    }
}