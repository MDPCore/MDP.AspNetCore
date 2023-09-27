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

namespace MDP.AspNetCore.Authentication
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        // Methods
        [AllowAnonymous]
        [Route("/.auth/signin", Name = "/.auth/signin")]
        public async Task<ActionResult> SignIn(string returnUrl = null)
        {
            // Require
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // AuthenticationProvider
            var authenticationProvider = this.HttpContext.RequestServices.GetService<AuthenticationProvider>();

            // AuthenticationSetting
            var authenticationSetting = this.HttpContext.RequestServices.GetService<DefaultAuthenticationSetting>();
            if (authenticationSetting == null) throw new InvalidOperationException($"{nameof(authenticationSetting)}=null");

            // RemoteIdentity
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity == null) throw new InvalidOperationException($"{nameof(remoteIdentity)}=null");

            // LocalIdentity
            var localIdentity = await this.LocalAuthenticateAsync();
            if (localIdentity != null && authenticationProvider == null) return this.Redirect(returnUrl);
            if (localIdentity != null && localIdentity.AuthenticationType == remoteIdentity.AuthenticationType) return this.Redirect(returnUrl);

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
                await this.HttpContext.RemoteSignOutAsync();
                await this.HttpContext.LocalSignInAsync(new ClaimsPrincipal(localIdentity));

                // Redirect
                return this.Redirect(returnUrl);
            }

            // Register
            if (localIdentity == null && string.IsNullOrEmpty(authenticationSetting.RegisterPath) == false)
            {
                // Sign
                await this.HttpContext.RemoteSignInAsync(new ClaimsPrincipal(remoteIdentity));
                await this.HttpContext.LocalSignOutAsync();

                // Register
                return this.Redirect(authenticationSetting.RegisterPath);
            }

            // Forbid
            {
                // Sign
                await this.HttpContext.RemoteSignOutAsync();
                await this.HttpContext.LocalSignOutAsync();

                // Forbid
                return this.Forbid();
            }
        }
    }
}