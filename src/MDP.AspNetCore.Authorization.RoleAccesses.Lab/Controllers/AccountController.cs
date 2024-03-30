using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MDP.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace MDP.AspNetCore.Authorization.RoleAccesses.Lab
{
    public partial class AccountController : Controller
    {
        // Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {
            // Return
            return await this.LogoutAsync();
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginByPassword(string username, string password, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(username) == true) throw new ArgumentException($"{nameof(username)}=null");
            //if (string.IsNullOrEmpty(password) == true) throw new ArgumentException($"{nameof(password)}=null");

            #endregion

            // ClaimsIdentity
            ClaimsIdentity claimsIdentity = null;

            // Clark(Admin) 
            if (claimsIdentity == null && username == "Clark")
            {
                claimsIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, "Clark"),
                    new Claim(ClaimTypes.Email, "Clark@hotmail.com"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("Nickname", "Clark")
                }, "Password");
            }

            // Other(User) 
            if (claimsIdentity == null)
            {
                claimsIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, $"{username}"),
                    new Claim(ClaimTypes.Email, $"{username}@hotmail.com"),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim("Nickname", $"{username}")
                }, "Password");
            }

            // Return
            return await this.LoginAsync(claimsIdentity, returnUrl);
        }
    }
}
