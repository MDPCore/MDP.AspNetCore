using MDP.AspNetCore.Authentication;
using MDP.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization.Lab
{
    [MDP.Registration.Service<IRoleAssignmentProvider>(singleton: true, autoRegister: true)]
    public class TenantAuthorizationProvider : IRoleAssignmentProvider
    {
        // Methods
        public List<RoleAssignment> Create(ClaimsIdentity claimsIdentity)
        {
            #region Contracts

            if (claimsIdentity == null) throw new ArgumentException($"{nameof(claimsIdentity)}=null");

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
                if(roleSectionArray==null) throw new InvalidOperationException($"{nameof(roleSectionArray)}=null");

                // RoleId
                var roleId = string.Empty;
                if (roleSectionArray.Length >= 1) roleId = roleSectionArray[0];
                if (string.IsNullOrEmpty(roleId) == true) continue;

                // TenantId
                var tenantId = string.Empty;
                if (roleSectionArray.Length >= 2) tenantId = roleSectionArray[1];

                // Scopes
                var scopes = new Dictionary<string, string>();
                if(string.IsNullOrEmpty(tenantId) == false) scopes.Add("TenantId", tenantId);

                // Add
                roleAssignmentList.Add(new RoleAssignment(roleId, scopes));
            }

            // Return
            return roleAssignmentList;
        }
    }
}