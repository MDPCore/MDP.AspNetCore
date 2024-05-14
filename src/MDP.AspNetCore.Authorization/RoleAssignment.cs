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
        public RoleAssignment(string roleId, Dictionary<string, string> scopes = null)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
          
            #endregion

            // Default
            this.RoleId = roleId;

            // Scopes
            if (scopes != null)
            {
                foreach (var scope in scopes)
                {
                    this.Scopes.Add(scope.Key, scope.Value);
                }
            }
        }


        // Properties
        public string RoleId { get; set; }

        public Dictionary<string, string> Scopes { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    }
}
