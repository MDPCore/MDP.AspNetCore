using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.RoleAccesses
{
    public class MemoryPermissionRepository : PermissionRepository
    {
        // Fields
        private List<Permission> _permissionList = null;


        // Constructors
        public MemoryPermissionRepository(List<Permission> permissionList)
        {
            #region Contracts

            if (permissionList == null) throw new ArgumentException($"{nameof(permissionList)}=null");

            #endregion

            // Default
            _permissionList = permissionList;
        }


        // Methods
        public List<Permission> FindAllByRoleId(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // FindAll
            return _permissionList.FindAll(o=>
                o.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase)==true && 
                o.AccessUri.AccessProvider.Equals(accessProvider, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessUri.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }
    }
}
