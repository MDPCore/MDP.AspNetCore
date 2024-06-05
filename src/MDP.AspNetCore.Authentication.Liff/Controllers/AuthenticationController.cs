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

namespace MDP.AspNetCore.Authentication.Liff
{
    public partial class AuthenticationController : Controller
    {
        // Fields
        private readonly IOptionsMonitor<LiffOptions> _optionsMonitor = null;


        // Constructors
        public AuthenticationController(IOptionsMonitor<LiffOptions> optionsMonitor)
        {
            #region Contracts

            if (optionsMonitor == null) throw new ArgumentNullException(nameof(optionsMonitor));

            #endregion

            // Default
            _optionsMonitor = optionsMonitor;
        }


        // Methods
        [AllowAnonymous]
        [Route("/.auth/login/liff/init")]
        public ActionResult Init(string returnUrl = null)
        {
            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // LiffOptions
            var liffOptions = _optionsMonitor.Get(LiffDefaults.AuthenticationScheme);
            if (liffOptions == null) throw new InvalidOperationException($"{nameof(liffOptions)}=null");
            if (liffOptions.LiffId == null) throw new InvalidOperationException($"{nameof(liffOptions.LiffId)}=null");

            // ViewBag
            this.ViewBag.LiffId = liffOptions.LiffId;
            this.ViewBag.ReturnUrl = returnUrl;

            // PrimaryURL
            if (this.Request.Query.ContainsKey("liff.state") == true)
            {
                // Return
                return View("_auth/liff/init");
            }

            // SecondaryURL: AutoLogin
            if (this.Request.Query.ContainsKey("autoLogin") == true)
            {
                // Redirect
                var autoLogin = this.Request.Query["autoLogin"].ToString().ToLower();
                if (autoLogin == "true") return this.Login(returnUrl);
            }

            // SecondaryURL: ReturnUrl
            if (this.Request.Query.ContainsKey("returnUrl") == true)
            {
                // Redirect
                return this.Redirect(returnUrl);
            }

            // Return
            return View("_auth/liff/init");
        }

        [AllowAnonymous]
        [Route("/.auth/login/liff/authorize")]
        public ActionResult Login(string returnUrl = null)
        {
            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // LiffOptions
            var liffOptions = _optionsMonitor.Get(LiffDefaults.AuthenticationScheme);
            if (liffOptions == null) throw new InvalidOperationException($"{nameof(liffOptions)}=null");
            if (liffOptions.LiffId == null) throw new InvalidOperationException($"{nameof(liffOptions.LiffId)}=null");

            // ViewBag
            this.ViewBag.LiffId = liffOptions.LiffId;
            this.ViewBag.ReturnUrl = returnUrl;

            // IsAuthenticated
            if (this.User?.Identity?.IsAuthenticated == true)
            {
                // Return
                return this.Redirect(returnUrl);
            }

            // Return
            return View("_auth/liff/authorize");
        }
    }
}