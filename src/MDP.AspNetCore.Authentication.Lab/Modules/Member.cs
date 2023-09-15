using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MDP.Members
{
    public class Member
    {
        // Properties
        public string MemberId { get; set; } = String.Empty;

        public string Name { get; set; } = String.Empty;

        public string Mail { get; set; } = String.Empty;

        public string Nickname { get; set; } = String.Empty;

        public Dictionary<string, string> Links { get; set; } = new Dictionary<string, string>();


        // Methods
        public Member Clone()
        {
            // Create
            var member = new Member();
            member.MemberId = this.MemberId;
            member.Name = this.Name;
            member.Mail = this.Mail;
            member.Nickname = this.Nickname;
            member.Links = this.Links.ToDictionary(o => o.Key, o => o.Value);

            // Return
            return member;
        }
    }
}
