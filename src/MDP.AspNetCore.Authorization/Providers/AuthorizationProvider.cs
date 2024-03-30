using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class AuthorizationProvider
    {
        // Methods
        public virtual List<RoleAssignment> FindAllRoleAssignment(ClaimsIdentity claimsIdentity)
        {
            #region Contracts

            if (claimsIdentity == null) throw new ArgumentException($"{nameof(claimsIdentity)}=null");

            #endregion

            // RoleIdList
            var roleIdList = claimsIdentity.Claims.Where(claim => claim.Type == claimsIdentity.RoleClaimType).Select(claim => claim.Value).ToList();
            if (roleIdList == null) throw new InvalidOperationException($"{nameof(roleIdList)}=null");

            //  RoleAssignmentList
            var roleAssignmentList = new List<RoleAssignment>();
            foreach (var roleId in roleIdList)
            {
                // Create
                var roleAssignment = new RoleAssignment(roleId);

                // Add
                roleAssignmentList.Add(roleAssignment);
            }

            // Return
            return roleAssignmentList;
        }

        public virtual List<Permission> FindAllPermission(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // Return
            return new List<Permission>();
        }
    }
}