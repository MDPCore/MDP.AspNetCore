using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace MDP.AspNetCore.Authentication.Lab
{
    public partial class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("/login", Name = "Login")]
        public ActionResult Login(string scheme = null, string returnUrl = null)
        {
            // Return
            return this.View("Login");
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginByName(string username, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(username) == true) throw new ArgumentException(nameof(username));

            #endregion

            // Require
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, username + "@hotmail.com"),
            }, "NameAuth");

            // SignIn
            await this.HttpContext.RemoteSignInAsync(new ClaimsPrincipal(claimsIdentity));

            // Return
            return this.RedirectToRoute("/.auth/signin", new { returnUrl = returnUrl });
        }
    }

    // GetUser
    public partial class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult<GetUserResultModel> GetUser([FromBody] GetUserActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // ClaimsIdentity
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");

            // UserModel
            var user = new UserModel();
            user.AuthenticationType = claimsIdentity.AuthenticationType!;
            user.UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            user.UserName = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value!;

            // Return
            return (new GetUserResultModel()
            {
                User = user
            });
        }


        // Class
        public class GetUserActionModel
        {
            // Properties

        }

        public class GetUserResultModel
        {
            // Properties
            public UserModel User { get; set; }
        }

        public class UserModel
        {
            // Properties
            public string AuthenticationType { get; set; } = string.Empty;

            public string UserId { get; set; } = string.Empty;

            public string UserName { get; set; } = string.Empty;
        }
    }
}
