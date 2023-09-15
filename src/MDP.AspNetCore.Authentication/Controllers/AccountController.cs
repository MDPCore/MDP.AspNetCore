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
        // Methods
        [AllowAnonymous]
        [Route("/.auth/login/{scheme}", Name = "/.auth/login")]
        public async Task<ActionResult> Login(string scheme = null, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");

            #endregion

            // Login
            return await this.LoginAsync(scheme, returnUrl);
        }

        [AllowAnonymous]
        [Route("/.auth/logout", Name = "/.auth/logout")]
        public async Task<ActionResult> Logout(string returnUrl = null)
        {
            // Logout
            return await this.LogoutAsync(returnUrl);
        }

        [AllowAnonymous]
        [Route("/.auth/signin", Name = "/.auth/signin")]
        public async Task<ActionResult> SignIn(string returnUrl = null)
        {
            // RemoteIdentity
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity == null) throw new InvalidOperationException($"{nameof(remoteIdentity)}=null");

            // Login
            return await this.LoginAsync(remoteIdentity, returnUrl);
        }
    }
}