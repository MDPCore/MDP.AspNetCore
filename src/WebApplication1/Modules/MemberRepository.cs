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


        // Constructors
        public MemberRepository()
        {
            // Default
            _memberList.Add(new Member() { MemberId = Guid.NewGuid().ToString(), Name = "Clark", Mail = "Clark@hotmail.com" });
            _memberList.Add(new Member() { MemberId = Guid.NewGuid().ToString(), Name = "Jane", Mail = "Jane@hotmail.com" });
        }


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

        public Member FindByName(string name)
        {
            #region Contracts

            if (string.IsNullOrEmpty(name) == true) throw new ArgumentException($"{nameof(name)}=null");

            #endregion

            // Return
            return _memberList.FirstOrDefault(o => o.Name == name)?.Clone();
        }

        public Member FindByMemberId(string memberId)
        {
            #region Contracts

            if (string.IsNullOrEmpty(memberId) == true) throw new ArgumentException($"{nameof(memberId)}=null");

            #endregion

            // Return
            return _memberList.FirstOrDefault(o => o.MemberId == memberId)?.Clone();
        }
    }
}