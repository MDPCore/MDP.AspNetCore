using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class DefaultRoleAssignmentProvider : IRoleAssignmentProvider
    {
        // Methods
        public List<RoleAssignment> Create(ClaimsIdentity claimsIdentity)
        {
            #region Contracts

            if (claimsIdentity == null) throw new ArgumentNullException($"{nameof(claimsIdentity)}=null");

            #endregion

            // RoleStringList
            var roleStringList = claimsIdentity.Claims.Where(claim => claim.Type == claimsIdentity.RoleClaimType).Select(claim => claim.Value).ToList();
            if (roleStringList == null) throw new InvalidOperationException($"{nameof(roleStringList)}=null");

            //  RoleAssignmentList
            var roleAssignmentList = new List<RoleAssignment>();
            foreach (var roleString in roleStringList)
            {
                // RoleSectionArray
                var roleSectionArray = roleString.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (roleSectionArray == null) throw new InvalidOperationException($"{nameof(roleSectionArray)}=null");
                if (roleSectionArray.Length == 0) throw new InvalidOperationException($"{nameof(roleSectionArray)}=null");

                // RoleId
                var roleId = roleSectionArray[roleSectionArray.Length - 1];
                if (string.IsNullOrEmpty(roleId) == true) throw new InvalidOperationException($"{nameof(roleId)}=null");

                // RoleScopes
                var roleScopes = roleSectionArray.Take(roleSectionArray.Length - 1).ToList();
                if (roleScopes == null) throw new InvalidOperationException($"{nameof(roleId)}=null");

                // Add
                roleAssignmentList.Add(new RoleAssignment(roleId, roleScopes));
            }

            // Return
            return roleAssignmentList;
        }
    }
}