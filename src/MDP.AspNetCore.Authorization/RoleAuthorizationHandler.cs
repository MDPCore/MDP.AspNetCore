using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequirement>
    {
        // Fields
        private readonly IList<IAccessResourceProvider> _accessResourceProviderList = null;

        private readonly IList<IRoleAssignmentProvider> _roleAssignmentProviderList = null;

        private readonly IList<IAccessPermissionProvider> _accessPermissionProviderList = null;


        // Constructors
        public RoleAuthorizationHandler(IList<IAccessResourceProvider> accessResourceProviderList, IList<IRoleAssignmentProvider> roleAssignmentProviderList, IList<IAccessPermissionProvider> accessPermissionProviderList)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(accessResourceProviderList);
            ArgumentNullException.ThrowIfNull(roleAssignmentProviderList);
            ArgumentNullException.ThrowIfNull(accessPermissionProviderList);

            #endregion

            // AccessResourceProviderList
            _accessResourceProviderList = accessResourceProviderList;

            // RoleAssignmentProviderList
            _roleAssignmentProviderList = roleAssignmentProviderList;
            if (_roleAssignmentProviderList.Count <= 0) _roleAssignmentProviderList.Add(new DefaultRoleAssignmentProvider());

            // AccessPermissionProviderList            
            _accessPermissionProviderList = accessPermissionProviderList;
        }


        // Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(requirement);

            #endregion

            // AccessResource
            AccessResource accessResource = null;
            foreach (var accessResourceProvider in _accessResourceProviderList)
            {
                // Create
                accessResource = accessResourceProvider.Create(context.Resource);
                if (accessResource != null) break;
            }
            if (accessResource == null) return Task.CompletedTask;

            // ClaimsIdentity
            var claimsIdentity = context.User?.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return Task.CompletedTask;
            if (claimsIdentity.IsAuthenticated == false) return Task.CompletedTask;

            // RoleAssignmentList
            var roleAssignmentList = new List<RoleAssignment>();
            foreach (var roleAssignmentProvider in _roleAssignmentProviderList)
            {
                // Create
                var roleAssignmentListSource = roleAssignmentProvider.Create(claimsIdentity);
                if (roleAssignmentListSource == null) throw new InvalidOperationException($"{nameof(roleAssignmentListSource)}=null");

                // Add
                roleAssignmentList.AddRange(roleAssignmentListSource);
            }

            // RoleAssignmentList.Foreach
            foreach (var roleAssignment in roleAssignmentList)
            {
                foreach (var accessPermissionProvider in _accessPermissionProviderList)
                {
                    // AccessPermissionList
                    var accessPermissionList = accessPermissionProvider.FindAll(roleAssignment.RoleId, roleAssignment.RoleScopes, accessResource.ResourceProvider, accessResource.ResourceType);
                    if (accessPermissionList == null) throw new InvalidOperationException($"{nameof(accessPermissionList)}=null");

                    // HasAccess
                    foreach (var accessPermission in accessPermissionList)
                    {
                        if (accessResource.HasAccess(roleAssignment, accessPermission) == true)
                        {
                            // Succeed
                            context.Succeed(requirement);

                            // Return
                            return Task.CompletedTask;
                        }
                    }
                }
            }

            // Return
            return Task.CompletedTask;
        }
    }
}
