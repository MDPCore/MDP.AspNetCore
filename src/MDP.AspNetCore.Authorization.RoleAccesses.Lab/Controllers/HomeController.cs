using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MDP.AspNetCore.Authorization.RoleAccesses.Lab
{
    public class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult Index()
        {
            // ViewBag
            this.ViewBag.Message = "Hello World";

            // Return
            return View();
        }
    }
}
