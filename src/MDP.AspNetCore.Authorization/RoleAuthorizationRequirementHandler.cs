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
    public class RoleAuthorizationRequirementHandler : AuthorizationHandler<RoleAuthorizationRequirement>
    {
        // Fields
        private readonly RoleAuthorizationRequirementHandlerSetting _authorizationSetting = null;

        private readonly AuthorizationProvider _authorizationProvider = null;


        // Constructors
        public RoleAuthorizationRequirementHandler(RoleAuthorizationRequirementHandlerSetting authorizationSetting, AuthorizationProvider authorizationProvider = null)
        {
            #region Contracts

            if (authorizationSetting == null) throw new ArgumentException($"{nameof(authorizationSetting)}=null");

            #endregion

            // Default
            _authorizationSetting = authorizationSetting;
            _authorizationProvider = authorizationProvider ?? new AuthorizationProvider();
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
                var permissionList = _authorizationSetting.FindAllPermission(roleAssignment.RoleId, resource.ResourceProvider, resource.ResourceType);
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
    }
}
