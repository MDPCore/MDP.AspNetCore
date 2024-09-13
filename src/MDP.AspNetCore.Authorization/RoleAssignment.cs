using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class RoleAssignment
    {
        // Constructors
        public RoleAssignment(string roleId) : this(roleId, new List<string>())
        {
            
        }

        public RoleAssignment(string roleId, List<string> roleScopes)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNullOrEmpty(roleId);
            ArgumentNullException.ThrowIfNull(roleScopes);

            #endregion

            // Default
            this.RoleId = roleId;
            this.RoleScopes = roleScopes;
        }


        // Properties
        public string RoleId { get; set; }

        public List<string> RoleScopes { get; set; }
    }
}
