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
        public Task<ActionResult> SignIn(string returnUrl = null)
        {
            // Return
            return this.SignInAsync(returnUrl);
        }
    }
}