using MDP.AspNetCore.Authentication.Line;
using MDP.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Lab
{
    public class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult Index()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public Task<ActionResult> LinkByLine(string returnUrl = null)
        {
            // Return
            return this.LinkAsync(LineDefaults.AuthenticationScheme, returnUrl);
        }
    }
}
