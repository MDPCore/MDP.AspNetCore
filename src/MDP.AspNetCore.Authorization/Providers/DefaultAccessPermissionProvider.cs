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

            ArgumentNullException.ThrowIfNull(accessPermissionList);

            #endregion

            // Default
            _accessPermissionList = accessPermissionList;
        }


        // Methods
        public List<AccessPermission> FindAll(string roleId, List<string> roleScopes, string accessProvider, string accessType)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNullOrEmpty(roleId);
            ArgumentNullException.ThrowIfNull(roleScopes);
            ArgumentNullException.ThrowIfNullOrEmpty(accessProvider);
            ArgumentNullException.ThrowIfNullOrEmpty(accessType);

            #endregion

            // Require
            if (_accessPermissionList.Count <= 0) return new List<AccessPermission>();

            // FindAll
            return _accessPermissionList.FindAll(o =>
                o.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase) == true &&
                o.RoleScopes.Count == roleScopes.Count &&
                o.AccessProvider.Equals(accessProvider, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }
    }
}
