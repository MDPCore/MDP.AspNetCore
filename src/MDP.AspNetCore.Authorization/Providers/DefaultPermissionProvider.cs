using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class DefaultPermissionProvider : IPermissionProvider
    {
        // Fields
        private readonly List<Permission> _permissionList = null;


        // Constructors
        public DefaultPermissionProvider(List<Permission> permissionList)
        {
            #region Contracts

            if (permissionList == null) throw new ArgumentException($"{nameof(permissionList)}=null");

            #endregion

            // Default
            _permissionList = permissionList;
        }


        // Methods
        public List<Permission> FindAll(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // Require
            if (_permissionList == null) return new List<Permission>();
            if (_permissionList.Count <= 0) return new List<Permission>();

            // FindAll
            return _permissionList.FindAll(o =>
                o.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessProvider.Equals(accessProvider, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }
    }
}
