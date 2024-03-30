using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MDP.AspNetCore.Authorization.Lab
{
    [Route("{tenantId}/{controller=Home}/{action=Index}")]
    public class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult Index(string tenantId)
        {
            // ViewBag
            this.ViewBag.Message = $"Hello World, tenantId={tenantId}";

            // Return
            return View();
        }
    }
}
