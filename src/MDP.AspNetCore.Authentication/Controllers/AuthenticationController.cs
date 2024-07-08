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
        [Route("/.auth/signin", Name = "/.auth/signin")]
        public async Task<ActionResult> SignIn(string returnUrl = null)
        {
            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // RemoteIdentity
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity == null) throw new InvalidOperationException($"{nameof(remoteIdentity)}=null");
            if (remoteIdentity.IsAuthenticated == false) throw new InvalidOperationException($"{nameof(remoteIdentity)}.IsAuthenticated=false");

            // SignIn
            return await this.SignInAsync(remoteIdentity, returnUrl);
        }        
    }
}