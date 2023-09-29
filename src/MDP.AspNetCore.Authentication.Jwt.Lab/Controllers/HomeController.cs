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
            // Return
            return View("Index");
        }
    }

    // GetToken
    public partial class HomeController : Controller
    {
        // Methods
        [AllowAnonymous]
        public ActionResult<GetTokenResultModel> GetToken([FromBody] GetTokenActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, actionModel.Username),
                new Claim(ClaimTypes.Email, actionModel.Username + "@hotmail.com"),
            }, "NameAuth");

            // TokenString
            var tokenString = _securityTokenFactory.CreateSecurityToken("RSAToken", claimsIdentity);
            if (string.IsNullOrEmpty(tokenString) == true) throw new InvalidOperationException($"{nameof(tokenString)}=null");

            // Return
            return (new GetTokenResultModel()
            {
                Token = tokenString
            });
        }


        // Class
        public class GetTokenActionModel
        {
            // Properties
            public string Username { get; set; } = string.Empty;
        }

        public class GetTokenResultModel
        {
            // Properties
            public string Token { get; set; } = string.Empty;
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