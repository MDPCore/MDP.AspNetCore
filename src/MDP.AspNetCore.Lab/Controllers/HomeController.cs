using MDP.Logging;
using MDP.Tracing;
using Microsoft.AspNetCore.Mvc;
using MyLab.Module;
using System;

namespace MDP.AspNetCore.Lab
{
    public class HomeController : Controller
    {
        // Fields
        private readonly WorkContext _workContext;


        // Constructors
        public HomeController(WorkContext workContext)
        {
            #region Contracts

            if (workContext == null) throw new ArgumentNullException($"{nameof(workContext)}=null");

            #endregion

            // Default
            _workContext = workContext;
        }


        // Methods
        public ActionResult Index()
        {
            // Message
            var message = _workContext.GetValue();
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
