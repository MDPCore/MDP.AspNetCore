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

namespace MDP.AspNetCore.Authentication.SecurityTokens.Lab
{
    // Base
    public partial class HomeController : Controller
    {
        // Fields
        private readonly SecurityTokenFactory _securityTokenFactory;


        // Constructors
        public HomeController(SecurityTokenFactory securityTokenFactory)
        {
            #region Contracts

            if (securityTokenFactory == null) throw new ArgumentException(nameof(securityTokenFactory));

            #endregion

            // Default
            _securityTokenFactory = securityTokenFactory;
        }


        // Methods
        public ActionResult Index()
        {
            // Variables
            var username = "Clark";

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(authenticationType: "Password", claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, $"{username}"),
                new Claim(ClaimTypes.Email, $"{username}@example.com"),
                new Claim(ClaimTypes.Role, "User")
            });

            // SecurityToken
            var securityToken = _securityTokenFactory.CreateToken(claimsIdentity);
            if (string.IsNullOrEmpty(securityToken) == true) throw new InvalidOperationException($"{nameof(securityToken)}=null");

            // ViewBag
            this.ViewBag.SecurityToken = securityToken;

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
            user.AuthenticationType = claimsIdentity.AuthenticationType;
            user.UserId = claimsIdentity.GetClaimValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            user.UserName = claimsIdentity.GetClaimValue(System.Security.Claims.ClaimTypes.Name);
            user.Mail = claimsIdentity.GetClaimValue(System.Security.Claims.ClaimTypes.Email);

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

            public string Mail { get; set; } = string.Empty;
        }
    }    
}