﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Facebook.Lab
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
        public Task<ActionResult> LoginByFacebook(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(FacebookDefaults.AuthenticationScheme, returnUrl);
        }
    }
}
