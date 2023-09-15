using MDP.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApplication1
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
    }
}
