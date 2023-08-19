using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace MDP.AspNetCore.Authentication.Liff.Services
{
    public class LiffController : Controller
    {
        // Fields
        private readonly IOptionsMonitor<LiffOptions> _optionsMonitor;


        // Constructors
        public LiffController(IOptionsMonitor<LiffOptions> optionsMonitor)
        {
            #region Contracts

            if (optionsMonitor == null) throw new ArgumentException(nameof(optionsMonitor));

            #endregion

            // Default
            _optionsMonitor = optionsMonitor;
        }


        // Methods
        [AllowAnonymous]
        [Route("/Login-Liff", Name = "Login-Liff")]
        public ActionResult Login(string authenticationScheme = null, string returnUrl = null)
        {
            // ReturnUrl
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (string.IsNullOrEmpty(returnUrl) == true) throw new InvalidOperationException($"{nameof(returnUrl)}=null");
          
            // AuthenticationScheme
            authenticationScheme = authenticationScheme ?? LiffDefaults.AuthenticationScheme;
            if (string.IsNullOrEmpty(authenticationScheme) == true) throw new InvalidOperationException($"{nameof(authenticationScheme)}=null");
                        
            // AuthenticationOptions
            var authenticationOptions = _optionsMonitor.Get(authenticationScheme);
            if (authenticationOptions == null) throw new InvalidOperationException($"{nameof(authenticationOptions)}=null");

            // ViewBag
            this.ViewBag.LiffId = authenticationOptions.LiffId;
            this.ViewBag.ReturnUrl = returnUrl;

            // IsAuthenticated
            if (this.User?.Identity?.IsAuthenticated == true)
            {
                // Redirect
                if (this.Request.Query.ContainsKey("liff.state") == false)
                {
                    return this.Redirect(returnUrl);
                }

                // LiffState
                var liffState = this.Request.Query?["liff.state"].FirstOrDefault();
                if (string.IsNullOrEmpty(liffState) == true) return this.Redirect(returnUrl);

                // ReturnUrl
                returnUrl = HttpUtility.ParseQueryString(liffState)?["returnUrl"];
                if (string.IsNullOrEmpty(returnUrl) == false) return this.Redirect(returnUrl);

                // Throw
                throw new InvalidOperationException($"{nameof(liffState)}={liffState}");
            }

            // Return
            return View("Login-Liff");
        }
    }
}