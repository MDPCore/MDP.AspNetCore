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
        private readonly List<Permission> _permissionList = null;

        private readonly AuthorizationProvider _authorizationProvider = null;


        // Constructors
        public RoleAuthorizationHandler(List<Permission> permissionList, AuthorizationProvider authorizationProvider)
        {
            #region Contracts

            if (permissionList == null) throw new ArgumentException($"{nameof(permissionList)}=null");
            if (authorizationProvider == null) throw new ArgumentException($"{nameof(authorizationProvider)}=null");

            #endregion

            // Default
            _permissionList = permissionList;
            _authorizationProvider = authorizationProvider;
        }


        // Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAuthorizationRequirement requirement)
        {
            #region Contracts

            if (context == null) throw new ArgumentException($"{nameof(context)}=null");
            if (requirement == null) throw new ArgumentException($"{nameof(requirement)}=null");

            #endregion

            // HttpContext
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null) return Task.CompletedTask;

            // Resource
            var resourceUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";
            var resource = new Resource(resourceUri);

            // ClaimsIdentity
            var claimsIdentity = httpContext.User?.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return Task.CompletedTask;
            if (claimsIdentity.IsAuthenticated == false) return Task.CompletedTask;

            // RoleAssignmentList
            var roleAssignmentList = _authorizationProvider.FindAllRoleAssignment(claimsIdentity);
            if (roleAssignmentList == null) throw new InvalidOperationException($"{nameof(roleAssignmentList)}=null");

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

            // LocalAuthorization
            {
                // PermissionList
                var permissionList = this.FindAllPermission(roleAssignment.RoleId, resource.ResourceProvider, resource.ResourceType);
                if (permissionList == null) throw new InvalidOperationException($"{nameof(permissionList)}=null");

                // HasAccess
                foreach (var permission in permissionList)
                {
                    if (permission.HasAccess(roleAssignment, resource) == true)
                    {
                        return true;
                    }
                }
            }

            // RemoteAuthorization
            {
                // PermissionList
                var permissionList = _authorizationProvider.FindAllPermission(roleAssignment.RoleId, resource.ResourceProvider, resource.ResourceType);
                if (permissionList == null) throw new InvalidOperationException($"{nameof(permissionList)}=null");

                // HasAccess
                foreach (var permission in permissionList)
                {
                    if (permission.HasAccess(roleAssignment, resource) == true)
                    {
                        return true;
                    }
                }
            }

            // Return
            return false;
        }

        private List<Permission> FindAllPermission(string roleId, string accessProvider, string accessType)
        {
            #region Contracts

            if (string.IsNullOrEmpty(roleId) == true) throw new ArgumentException($"{nameof(roleId)}=null");
            if (string.IsNullOrEmpty(accessProvider) == true) throw new ArgumentException($"{nameof(accessProvider)}=null");
            if (string.IsNullOrEmpty(accessType) == true) throw new ArgumentException($"{nameof(accessType)}=null");

            #endregion

            // Require
            if (_permissionList == null) return new List<Permission>();
            if (_permissionList.Count <= 0) return new List<Permission>();

            // FindAll
            return _permissionList.FindAll(o =>
                o.RoleId.Equals(roleId, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessProvider.Equals(accessProvider, StringComparison.OrdinalIgnoreCase) == true &&
                o.AccessType.Equals(accessType, StringComparison.OrdinalIgnoreCase) == true
            ).ToList();
        }
    }
}
