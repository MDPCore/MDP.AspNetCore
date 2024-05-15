using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class DefaultAccessPermissionProvider : IAccessPermissionProvider
    {
        // Fields
        private readonly List<AccessPermission> _accessPermissionList = null;


        // Constructors
        public DefaultAccessPermissionProvider(List<AccessPermission> accessPermissionList)
        {
            #region Contracts

            if (accessPermissionList == null) throw new ArgumentException($"{nameof(accessPermissionList)}=null");

            #endregion

            // Default
            _accessPermissionList = accessPermissionList;
        }


        // Methods
        public List<AccessPermission> Create(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // Require
            if (_accessPermissionList == null) return new List<AccessPermission>();
            if (_accessPermissionList.Count <= 0) return new List<AccessPermission>();

            // FindAll
            return _accessPermissionList.FindAll(o =>
                o.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessProvider.Equals(accessProvider, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }
    }
}
