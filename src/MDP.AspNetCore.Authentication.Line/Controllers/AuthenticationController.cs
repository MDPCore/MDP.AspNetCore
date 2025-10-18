using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using static System.Net.WebRequestMethods;

namespace MDP.AspNetCore.Authentication.Line
{
    public partial class AuthenticationController : Controller
    {
        // Fields
        private readonly IOptionsMonitor<LineOptions> _optionsMonitor = null;


        // Constructors
        public AuthenticationController(IOptionsMonitor<LineOptions> optionsMonitor)
        {
            #region Contracts

            if (optionsMonitor == null) throw new ArgumentNullException(nameof(optionsMonitor));

            #endregion

            // Default
            _optionsMonitor = optionsMonitor;
        }


        // Methods
        [Authorize]
        [Route("/.auth/login/line/check")]
        public async Task<ActionResult> Check(string returnUrl = null)
        {
            // Require
            returnUrl = this.NormalizeReturnUrl(returnUrl);

            // Friend
            var isFriend = !(this.User.FindFirst("mdp:line:friend")?.Value == "false");
            if (isFriend == true) return this.Redirect(returnUrl);

            // SignOut
            await this.HttpContext.RemoteSignOutAsync();
            await this.HttpContext.LocalSignOutAsync();

            // ReturnUrl
            returnUrl = QueryHelpers.AddQueryString("/.auth/login/line/check", "returnUrl", returnUrl);
            if (string.IsNullOrEmpty(returnUrl) == true) throw new InvalidOperationException($"{nameof(returnUrl)}=null");

            // Return
            return await this.LoginAsync(LineDefaults.AuthenticationScheme, returnUrl);
        }
    }
}