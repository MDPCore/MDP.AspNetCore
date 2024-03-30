using MDP.RoleAccesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization.RoleAccesses
{
    public class RoleAccessRequirementHandler : AuthorizationHandler<RoleAccessRequirement>
    {
        // Fields
        private readonly RoleAccessesContext _accessesContext = null;


        // Constructors
        public RoleAccessRequirementHandler(RoleAccessesContext accessesContext)
        {
            #region Contracts

            if (accessesContext == null) throw new ArgumentException($"{nameof(accessesContext)}=null");

            #endregion

            // Default
            _accessesContext = accessesContext;
        }


        // Methods
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleAccessRequirement requirement)
        {
            #region Contracts

            if (context == null) throw new ArgumentException($"{nameof(context)}=null");
            if (requirement == null) throw new ArgumentException($"{nameof(requirement)}=null");

            #endregion

            // HttpContext
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null) return Task.CompletedTask;

            // Identity
            var identity = httpContext.User?.Identity as ClaimsIdentity;
            if (identity == null) return Task.CompletedTask;
            if (identity.IsAuthenticated == false) return Task.CompletedTask;

            // RoleList
            var roleList = identity.Claims.Where(claim => claim.Type == identity.RoleClaimType).Select(claim => claim.Value).ToList();
            if (roleList == null) return Task.CompletedTask;
            if (roleList.Count == 0) return Task.CompletedTask;

            // ResourceUri
            var resourceUri = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}";

            // HasAccess
            if (_accessesContext.HasAccess(roleList, resourceUri) == true)
            {
                context.Succeed(requirement);
            }

            // Return
            return Task.CompletedTask;
        }
    }
}
