using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authentication.Line;
using MDP.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Lab
{
    public partial class AccountController : Controller
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
        public Task<ActionResult> LoginByLine(string returnUrl = null)
        {
            // Return
            return this.LoginAsync(LineDefaults.AuthenticationScheme, returnUrl);
        }

        [AllowAnonymous]
        public async Task<ActionResult> LoginByPassword(string username, string password, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(username) == true) throw new ArgumentException($"{nameof(username)}=null");
            //if (string.IsNullOrEmpty(password) == true) throw new ArgumentException($"{nameof(password)}=null");

            #endregion

            // Member
            var member = _memberRepository.FindByPassword(username, password);
            if (member == null)
            {
                // Message
                this.ViewBag.Message = "Login failed";

                // Return
                return this.View("Login");
            }

            // Return
            return await this.LoginAsync(member.ToIdentity("Password"), returnUrl);
        }
    }

    public partial class AccountController : Controller
    {
        // Methods
        [AllowAnonymous]
        public async Task<ActionResult> Register()
        {
            // DefaultValue
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity != null)
            {                
                this.ViewBag.Name = remoteIdentity?.GetClaimValue(ClaimTypes.Name);
                this.ViewBag.Mail = remoteIdentity?.GetClaimValue(ClaimTypes.Email);
                this.ViewBag.Nickname = remoteIdentity?.GetClaimValue(ClaimTypes.Name);
            }

            // Return
            return this.View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> RegisterMember(string name, string mail, string nickname, string password, string returnUrl = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentException($"{nameof(name)}=null");
            if (string.IsNullOrEmpty(mail) == true) throw new ArgumentException($"{nameof(mail)}=null");
            //if (string.IsNullOrEmpty(nickname) == true) throw new ArgumentException($"{nameof(nickname)}=null");
            //if (string.IsNullOrEmpty(password) == true) throw new ArgumentException($"{nameof(password)}=null");

            #endregion

            // Member
            var member = new Member();
            member.MemberId = Guid.NewGuid().ToString();
            member.Name = name;
            member.Mail = mail;
            member.Nickname = nickname;

            // MemberLink
            var remoteIdentity = await this.RemoteAuthenticateAsync();
            if (remoteIdentity != null)
            {
                var linkType = remoteIdentity.AuthenticationType;
                var linkId = remoteIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
                member.Links.Add(linkType, linkId);
            }

            // Add
            _memberRepository.Add(member);

            // Return
            return await this.LoginAsync(member.ToIdentity("Password"), returnUrl);
        }
    }
}
