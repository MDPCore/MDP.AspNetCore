using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Line.Lab
{
    public class AccountController : Controller
    {
        // Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public Task<ActionResult> Logout()
        {
            // Return
            return this.LogoutAsync();
        }

        [AllowAnonymous]
        public Task<ActionResult> LoginByLine(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(LineDefaults.AuthenticationScheme, returnUrl);
        }
    }
}
