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
        // Methods
        [HttpGet]
        [AllowAnonymous]
        [Route("/.auth/login/{scheme}", Name = "/.auth/login")]
        public Task<ActionResult> Login(string scheme, string returnUrl = null)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNullOrEmpty(scheme);

            #endregion

            // Login
            return this.LoginAsync(scheme, returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/.auth/link/{scheme}", Name = "/.auth/link")]
        public Task<ActionResult> Link(string scheme, string returnUrl = null)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNullOrEmpty(scheme);

            #endregion

            // Link
            return this.LinkAsync(scheme, returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/.auth/refresh", Name = "/.auth/refresh")]
        public Task<ActionResult> Refresh(string returnUrl = null)
        {
            // Refresh
            return this.RefreshAsync(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/.auth/logout", Name = "/.auth/logout")]
        public Task<ActionResult> Logout(string returnUrl = null)
        {
            // Logout
            return this.LogoutAsync(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/.auth/signin", Name = "/.auth/signin")]
        public Task<ActionResult> SignIn(string returnUrl = null)
        {
            // SignIn
            return this.SignInAsync(returnUrl);
        }        
    }
}