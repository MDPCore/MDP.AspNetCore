using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MDP.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using MDP.Security.Tokens.Jwt;

namespace MDP.AspNetCore.Authentication.Jwt.Lab
{
    // Base
    public partial class HomeController : Controller
    {
        // Fields
        private readonly SecurityTokenFactory _securityTokenFactory;


        // Constructors
        public HomeController(SecurityTokenFactory tokenFactory)
        {
            #region Contracts

            if (tokenFactory == null) throw new ArgumentException(nameof(tokenFactory));

            #endregion

            // Default
            _securityTokenFactory = tokenFactory;
        }


        // Methods
        public ActionResult Index()
        {
            // Variables
            var username = "Clark";

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(System.Security.Claims.ClaimTypes.Name, username),
                new Claim(System.Security.Claims.ClaimTypes.Email, username + "@hotmail.com"),
            }, "Password");

            // HmacToken
            var hmacToken = _securityTokenFactory.CreateSecurityToken("HmacToken", claimsIdentity);
            if (string.IsNullOrEmpty(hmacToken) == true) throw new InvalidOperationException($"{nameof(hmacToken)}=null");
            this.ViewBag.HmacToken = hmacToken;

            // RsaToken
            var rsaToken = _securityTokenFactory.CreateSecurityToken("RsaToken", claimsIdentity);
            if (string.IsNullOrEmpty(rsaToken) == true) throw new InvalidOperationException($"{nameof(rsaToken)}=null");
            this.ViewBag.RsaToken = rsaToken;

            // Return
            return View("Index");
        }
    }

    // GetUser
    public partial class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult<UserModel> GetUser()
        {
            // ClaimsIdentity
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");

            // UserModel
            var user = new UserModel();
            user.AuthenticationType = claimsIdentity.AuthenticationType!;
            user.UserId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            user.UserName = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value!;

            // Return
            return user;
        }


        // Class
        public class UserModel
        {
            // Properties
            public string AuthenticationType { get; set; } = string.Empty;

            public string UserId { get; set; } = string.Empty;

            public string UserName { get; set; } = string.Empty;
        }
    }    
}