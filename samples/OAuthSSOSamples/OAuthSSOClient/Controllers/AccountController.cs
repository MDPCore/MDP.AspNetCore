﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.OAuthSSO;

namespace OAuthSSOClient
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
        public Task<ActionResult> LoginByOAuthSSO(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(OAuthSSODefaults.AuthenticationScheme, returnUrl);
        }
    }
}
