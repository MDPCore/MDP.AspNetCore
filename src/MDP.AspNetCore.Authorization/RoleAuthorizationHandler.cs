using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public class RoleAuthorizationHandler : AuthorizationHandler<RoleAuthorizationRequirement>
    {
        // Fields
        private readonly IList<IPermissionProvider> _permissionProviderList = null;

        private readonly IList<IRoleAssignmentProvider> _roleAssignmentProviderList = null;

        private readonly IResourceProvider _resourceProvider = null;


        // Constructors
        public RoleAuthorizationHandler(IList<IPermissionProvider> permissionProviderList, IList<IRoleAssignmentProvider> roleAssignmentProviderList, IResourceProvider resourceProvider =null)
        {
            #region Contracts

            if (permissionProviderList == null) throw new ArgumentException($"{nameof(permissionProviderList)}=null");
            if (roleAssignmentProviderList == null) throw new ArgumentException($"{nameof(roleAssignmentProviderList)}=null");

            #endregion

            // Default
            _permissionProviderList= permissionProviderList;
            _roleAssignmentProviderList = roleAssignmentProviderList;
            _resourceProvider = resourceProvider;

            // RoleAssignmentProviderList
            if (roleAssignmentProviderList.Count <= 0)
            {
                roleAssignmentProviderList.Add(new DefaultRoleAssignmentProvider());
            }
        }


        // Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            #region Contracts

            if (context == null) throw new ArgumentException($"{nameof(context)}=null");
            if (requirement == null) throw new ArgumentException($"{nameof(requirement)}=null");

            #endregion

            // ResourceProvider
            IResourceProvider resourceProvider = null;
            if (context.Resource == null) resourceProvider = _resourceProvider;
            if (context.Resource != null) resourceProvider = new DefaultResourceProvider(context);
            if (resourceProvider == null) return Task.CompletedTask;

            // Resource
            var resource = resourceProvider.Create();
            if (resource == null) return Task.CompletedTask;

            // ClaimsIdentity
            var claimsIdentity = context.User?.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return Task.CompletedTask;
            if (claimsIdentity.IsAuthenticated == false) return Task.CompletedTask;

            // RoleAssignmentList
            var roleAssignmentList = new List<RoleAssignment>();
            foreach (var roleAssignmentProvider in _roleAssignmentProviderList)
            {
                // FindAll
                var roleAssignmentListSource = roleAssignmentProvider.FindAll(claimsIdentity);
                if (roleAssignmentListSource == null) throw new InvalidOperationException($"{nameof(roleAssignmentListSource)}=null");

                // Add
                roleAssignmentList.AddRange(roleAssignmentListSource);
            }

            // RoleAssignmentList.ForEach
            foreach (var roleAssignment in roleAssignmentList)
            {
                // HasAccess
                if (this.HasAccess(roleAssignment, resource) == true)
                {
                    // Succeed
                    context.Succeed(requirement);

                    // Return
                    return Task.CompletedTask;
                }
            }

            // Return
            return Task.CompletedTask;
        }

        private bool HasAccess(RoleAssignment roleAssignment, Resource resource)
        {
            #region Contracts

            if (roleAssignment == null) throw new ArgumentException($"{nameof(roleAssignment)}=null");
            if (resource == null) throw new ArgumentException($"{nameof(resource)}=null");

            #endregion

            // PermissionList
            var permissionList = new List<Permission>();
            foreach (var permissionProvider in _permissionProviderList)
            {
                // FindAll
                var permissionListSource = permissionProvider.FindAll(roleAssignment.RoleId, resource.ResourceProvider, resource.ResourceType);
                if (permissionListSource == null) throw new InvalidOperationException($"{nameof(permissionListSource)}=null");

                // Add
                permissionList.AddRange(permissionListSource);
            }

            // HasAccess
            foreach (var permission in permissionList)
            {
                if (permission.HasAccess(roleAssignment, resource) == true)
                {
                    return true;
                }
            }

            // Return
            return false;
        }
    }
}
