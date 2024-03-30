using MDP.Navigation;
using Microsoft.AspNetCore.Mvc;
using System;

namespace MDP.AspNetCore.Navigation.Lab
{
    public class HomeController : Controller
    {
        // Fields
        private readonly NavigationContext _navigationContext = null;


        // Constructors
        public HomeController(NavigationContext navigationContext)
        {
            #region Contracts

            if (navigationContext == null) throw new ArgumentException($"{nameof(navigationContext)}=null");

            #endregion

            // Default
            _navigationContext = navigationContext;
        }


        // Methods
        public ActionResult Index()
        {
            // ViewBag
            this.ViewBag.MenuNodeList = _navigationContext.FindAllMenuNode();

            // Return
            return View();
        }
    }
}
