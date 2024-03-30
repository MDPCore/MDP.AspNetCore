using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class RoleAuthorizationRequirementHandlerSetting
    {
        // Properties
        public List<Permission> PermissionList { get; set; } = null;


        // Methods
        internal List<Permission> FindAllPermission(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // Require
            if (this.PermissionList == null) return new List<Permission>();
            if (this.PermissionList.Count <= 0) return new List<Permission>();

            // FindAll
            return this.PermissionList.FindAll(o =>
                o.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessProvider.Equals(accessProvider, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }
    }
}
