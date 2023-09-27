using MDP.AspNetCore.Authentication;
using MDP.Members;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MDP.Security.Claims;
using System.Xml.Schema;

namespace MDP.Members
{
    [MDP.Registration.Service<AuthenticationProvider>(singleton: true)]
    public class MemberAuthenticationProvider : AuthenticationProvider
    {
        // Fields
        private readonly MemberRepository _memberRepository;


        // Constructors
        public MemberAuthenticationProvider(MemberRepository memberRepository)
        {
            #region Contracts

            if (memberRepository == null) throw new ArgumentException($"{nameof(memberRepository)}=null");

            #endregion

            // Default
            _memberRepository = memberRepository;
        }


        // Methods
        public override ClaimsIdentity RemoteExchange(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // Member
            var linkType = remoteIdentity.AuthenticationType;
            var linkId = remoteIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
            var member = _memberRepository.FindByLink(linkType, linkId);
            if (member == null) return null;

            // Return
            return member.ToIdentity(remoteIdentity.AuthenticationType);
        }

        public override void RemoteLink(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");
            if (localIdentity == null) throw new ArgumentException($"{nameof(localIdentity)}=null");

            #endregion

            // Member
            var memberId = localIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
            var member = _memberRepository.FindByMemberId(memberId);
            if (member == null) throw new InvalidOperationException($"{nameof(member)}=null");

            // MemberLink
            var linkType = remoteIdentity.AuthenticationType;
            var linkId = remoteIdentity.GetClaimValue(ClaimTypes.NameIdentifier);
            member.Links.Remove(linkType);
            member.Links.Add(linkType, linkId);

            // Update
            _memberRepository.Update(member);
        }
    }
}
