using Microsoft.AspNetCore.Mvc;
using System;

namespace MDP.AspNetCore.DataProtection.MSSql.Lab
{
    public class HomeController : Controller
    {
        // Methods
        public ActionResult Index()
        {
            // Message
            var message = "Hello World";
            if (string.IsNullOrEmpty(message) == true) throw new InvalidOperationException($"{nameof(message)}=null");

            // ViewBag
            this.ViewBag.Message = message;

            // Return
            return View();
        }

        public ActionResult<string> Echo(string value)
        {
            #region Contracts

            if (string.IsNullOrEmpty(value) == true) throw new ArgumentNullException($"{nameof(value)}=null");

            #endregion

            // Return
            return value;
        }
    }
}
