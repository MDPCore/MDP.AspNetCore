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
        // Singleton
        private static readonly object _syncRoot = new object();

        private static DefaultRoleAssignmentProvider _instance = null;

        public static DefaultRoleAssignmentProvider Instance
        {
            get
            {
                // Require
                if (_instance != null) return _instance;

                // Create
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new DefaultRoleAssignmentProvider();
                    }
                }

                // Return
                return _instance;
            }
        }


        // Methods
        public List<RoleAssignment> Create(ClaimsIdentity claimsIdentity)
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
    }
}