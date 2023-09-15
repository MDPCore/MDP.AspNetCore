using MDP.AspNetCore.Authentication;
using MDP.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Lab
{
    public class AccountController : Controller
    {
        // Fields
        private readonly MemberRepository _memberRepository;


        // Constructors
        public AccountController(MemberRepository memberRepository)
        {
            #region Contracts

            if (memberRepository == null) throw new ArgumentException($"{nameof(memberRepository)}=null");

            #endregion

            // Default
            _memberRepository = memberRepository;
        }


        // Methods
        [AllowAnonymous]
        public ActionResult Login()
        {
            // Return
            return this.View();
        }

        [AllowAnonymous]
        public Task<ActionResult> Logout()
        {
            // Return
            return this.LogoutAsync();
        }

        [AllowAnonymous]
        public Task<ActionResult> LoginByPassword(string username, string password, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(username) == true) throw new ArgumentException($"{nameof(username)}=null");
            //if (string.IsNullOrEmpty(password) == true) throw new ArgumentException($"{nameof(password)}=null");

            #endregion

            // Ckeck Username + Password (for demo)
            var member = _memberRepository.FindByName(username);
            if (member == null)
            {
                // Message
                this.ViewBag.Message = "Login failed";

                // Return
                return Task.Run<ActionResult>(()=>this.View("Login"));
            }

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, member.MemberId),
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.Email, member.Mail),
            }, "Password");

            // Return
            return this.LoginAsync(claimsIdentity, returnUrl);
        }
    }
}
