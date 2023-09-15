using MDP.AspNetCore.Authentication;
using MDP.Members;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public override ClaimsIdentity Login(ClaimsIdentity remoteIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");

            #endregion

            // Return
            return base.Login(remoteIdentity);
        }

        public override ClaimsIdentity Link(ClaimsIdentity remoteIdentity, ClaimsIdentity localIdentity)
        {
            #region Contracts

            if (remoteIdentity == null) throw new ArgumentException($"{nameof(remoteIdentity)}=null");
            if (localIdentity == null) throw new ArgumentException($"{nameof(localIdentity)}=null");

            #endregion

            // Return
            return base.Link(remoteIdentity, localIdentity);
        }
    }
}
