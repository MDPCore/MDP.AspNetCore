using MDP.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MDP.Members
{
    public static class MemberExtensions
    {
        // Methods
        public static ClaimsIdentity ToIdentity(this Member member, string authenticationType)
        {
            #region Contracts

            if (member == null) throw new ArgumentException($"{nameof(member)}=null");
            if (string.IsNullOrEmpty(authenticationType) == true) throw new ArgumentException($"{nameof(authenticationType)}=null");

            #endregion

            // ClaimsIdentity
            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, member.MemberId),
                new Claim(ClaimTypes.Name, member.Name),
                new Claim(ClaimTypes.Email, member.Mail),
                new Claim("Nickname", member.Nickname)
            }, authenticationType);

            // Links
            var linksValue = string.Empty;
            foreach (var link in member.Links)
            {
                linksValue += $"{link.Key}:{link.Value};";
            }
            claimsIdentity.AddClaim(new Claim("Links", linksValue));

            // Return
            return claimsIdentity;
        }
    }
}