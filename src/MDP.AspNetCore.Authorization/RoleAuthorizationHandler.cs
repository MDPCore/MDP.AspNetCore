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
        private readonly IList<IRoleAssignmentProvider> _roleAssignmentProviderList = null;

        private readonly IList<IAccessPermissionProvider> _accessPermissionProviderList = null;

        private readonly IAccessResourceProvider _accessResourceProvider = null;


        // Constructors
        public RoleAuthorizationHandler(IList<IRoleAssignmentProvider> roleAssignmentProviderList, IList<IAccessPermissionProvider> accessPermissionProviderList, IAccessResourceProvider accessResourceProvider = null)
        {
            #region Contracts

            if (roleAssignmentProviderList == null) throw new ArgumentException($"{nameof(roleAssignmentProviderList)}=null");
            if (accessPermissionProviderList == null) throw new ArgumentException($"{nameof(accessPermissionProviderList)}=null");

            #endregion

            // RoleAssignmentProviderList
            _roleAssignmentProviderList = roleAssignmentProviderList;
            if (_roleAssignmentProviderList.Count <= 0)
            {
                _roleAssignmentProviderList.Add(DefaultRoleAssignmentProvider.Instance);
            }

            // AccessPermissionProviderList            
            _accessPermissionProviderList = accessPermissionProviderList;

            // AccessResourceProvider
            _accessResourceProvider = accessResourceProvider;
        }


        // Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            #region Contracts

            if (context == null) throw new ArgumentException($"{nameof(context)}=null");
            if (requirement == null) throw new ArgumentException($"{nameof(requirement)}=null");

            #endregion

            // AccessResourceProvider
            IAccessResourceProvider accessResourceProvider = null;
            if (context.Resource == null) accessResourceProvider = _accessResourceProvider;
            if (context.Resource != null) accessResourceProvider = new HttpAccessResourceProvider(context);
            if (accessResourceProvider == null) return Task.CompletedTask;

            // AccessResource
            var accessResource = accessResourceProvider.Create();
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
                // HasAccess
                if (this.HasAccess(roleAssignment, accessResource) == true)
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

        private bool HasAccess(RoleAssignment roleAssignment, AccessResource accessResource)
        {
            #region Contracts

            if (roleAssignment == null) throw new ArgumentException($"{nameof(roleAssignment)}=null");
            if (accessResource == null) throw new ArgumentException($"{nameof(accessResource)}=null");

            #endregion

            // AccessPermission
            var accessPermissionList = new List<AccessPermission>();
            foreach (var accessPermissionProvider in _accessPermissionProviderList)
            {
                // Create
                var accessPermissionListSource = accessPermissionProvider.Create(roleAssignment.RoleId, accessResource.ResourceProvider, accessResource.ResourceType);
                if (accessPermissionListSource == null) throw new InvalidOperationException($"{nameof(accessPermissionListSource)}=null");

                // Add
                accessPermissionList.AddRange(accessPermissionListSource);
            }

            // HasAccess
            foreach (var accessPermission in accessPermissionList)
            {
                if (accessPermission.HasAccess(roleAssignment, accessResource) == true)
                {
                    return true;
                }
            }

            // Return
            return false;
        }
    }
}
