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

namespace MDP.AspNetCore.Authentication.Tokens.Lab
{
    // Base
    public partial class HomeController : Controller
    {
        // Fields
        private readonly TokenProviderFactory _tokenProviderFactory;


        // Constructors
        public HomeController(TokenProviderFactory tokenProviderFactory)
        {
            #region Contracts

            if (tokenProviderFactory == null) throw new ArgumentNullException(nameof(tokenProviderFactory));

            #endregion

            // Default
            _tokenProviderFactory = tokenProviderFactory;
        }


        // Methods
        public ActionResult Index()
        {
            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(authenticationType: "TestAuth", claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "Clark"),
                new Claim(ClaimTypes.Email, "Clark@example.com"),
                new Claim(ClaimTypes.Role, "User")
            });

            // JwtBearer
            {
                // TokenProvider
                var tokenProvider = _tokenProviderFactory.CreateProvider("JwtBearer");
                if (tokenProvider == null) throw new InvalidOperationException($"{nameof(tokenProvider)}=null");

                // Token
                var token = tokenProvider.CreateToken(claimsIdentity);
                if (string.IsNullOrEmpty(token) == true) throw new InvalidOperationException($"{nameof(token)}=null");

                // ViewBag
                this.ViewBag.JwtBearer = token;
            }

            // ApiToken
            {
                // TokenProvider
                var tokenProvider = _tokenProviderFactory.CreateProvider("ApiToken");
                if (tokenProvider == null) throw new InvalidOperationException($"{nameof(tokenProvider)}=null");

                // Token
                var token = tokenProvider.CreateToken(claimsIdentity);
                if (string.IsNullOrEmpty(token) == true) throw new InvalidOperationException($"{nameof(token)}=null");

                // ViewBag
                this.ViewBag.ApiToken = token;
            }

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