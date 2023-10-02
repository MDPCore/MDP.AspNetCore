using MDP.Registration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.Members
{
    [Service<MemberRepository>(singleton: true)]
    public class MemberRepository
    {
        // Fields
        private readonly List<Member> _memberList = new List<Member>();


        // Methods
        public void Add(Member member)
        {
            #region Contracts

            if (member == null) throw new ArgumentException($"{nameof(member)}=null");

            #endregion

            // Add
            _memberList.RemoveAll(o => o.MemberId == member.MemberId);
            _memberList.Add(member);
        }

        public void Update(Member member)
        {
            #region Contracts

            if (member == null) throw new ArgumentException($"{nameof(member)}=null");

            #endregion

            // Update
            _memberList.RemoveAll(o => o.MemberId == member.MemberId);
            _memberList.Add(member);
        }

        public Member FindByMemberId(string memberId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(memberId) == true) throw new ArgumentException($"{nameof(memberId)}=null");

            #endregion

            // Return
            return _memberList.FirstOrDefault(o => o.MemberId == memberId)?.Clone();
        }

        public Member FindByPassword(string username, string password)
        {
            #region Contracts

            if (string.IsNullOrEmpty(username) == true) throw new ArgumentException($"{nameof(username)}=null");
            //if (string.IsNullOrEmpty(password) == true) throw new ArgumentException($"{nameof(password)}=null");

            #endregion

            // Return
            return _memberList.FirstOrDefault(o => o.Name == username)?.Clone();
        }

        public Member FindByLink(string linkType, string linkId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(linkType) == true) throw new ArgumentException($"{nameof(linkType)}=null");
            if (string.IsNullOrEmpty(linkId) == true) throw new ArgumentException($"{nameof(linkId)}=null");

            #endregion

            // Return
            return _memberList.FirstOrDefault(o => o.Links.ContainsKey(linkType) && o.Links[linkType] == linkId)?.Clone();
        }
    }
}