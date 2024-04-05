using MDP.AspNetCore.Authentication.Line;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Liff.Lab
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
        public Task<ActionResult> Logout(string returnUrl = null)
        {
            // Return
            return this.LogoutAsync(returnUrl);
        }

        [AllowAnonymous]
        public Task<ActionResult> LoginByLiff(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(LiffDefaults.AuthenticationScheme, returnUrl);
        }
    }
}
