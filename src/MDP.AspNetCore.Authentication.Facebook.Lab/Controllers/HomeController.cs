using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace MDP.AspNetCore.Authentication.Facebook.Lab
{
    public class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult Index()
        {
            // Return
            return View("Index");
        }

        [AllowAnonymous]
        [Route("/login", Name = "Login")]
        public ActionResult Login(string scheme = null, string returnUrl = null)
        {
            // Return
            return this.View("Login");
        }
    }
}
