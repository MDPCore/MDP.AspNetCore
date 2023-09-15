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