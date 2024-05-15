using MDP.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.AspNetCore.Authorization
{
    public class AuthorizationFactory : ServiceFactory<WebApplicationBuilder, AuthorizationFactory.Setting>
    {
        // Constructors
        public AuthorizationFactory() : base("Authorization", null, true) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AuthorizationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Authorization
            applicationBuilder.Services.AddAuthorization(options =>
            {
                // RequirementList
                var requirementList = options.DefaultPolicy?.Requirements.ToList();
                if (requirementList == null) requirementList = new List<IAuthorizationRequirement>();

                // PolicyBuilder
                var policyBuilder = new AuthorizationPolicyBuilder();
                foreach (var requirement in requirementList)
                {
                    policyBuilder.AddRequirements(requirement);
                }
                policyBuilder.AddRequirements(new RoleAuthorizationRequirement());

                // DefaultPolicy
                options.DefaultPolicy = policyBuilder.Build();
            });

            // RoleAuthorizationHandler
            applicationBuilder.Services.AddScoped<IAuthorizationHandler, RoleAuthorizationHandler>();

            // RoleAssignmentProvider

            // AccessPermissionProvider
            applicationBuilder.Services.AddSingleton<IAccessPermissionProvider>(serviceProvider =>
            {
                // AccessPermissionList
                var accessPermissionList = setting.Permissions?.Select(o => o.ToPermission()).ToList();
                if (accessPermissionList == null) accessPermissionList = new List<AccessPermission>();

                // Return
                return new DefaultAccessPermissionProvider(accessPermissionList);
            });

            // AccessResourceProvider

        }


        // Class
        public class Setting
        {
            // Properties
            public List<PermissionSetting> Permissions { get; set; } = null;
        }

        public class PermissionSetting
        {
            // Properties
            public string RoleId { get; set; }

            public string AccessUri { get; set; }


            // Methods
            public AccessPermission ToPermission()
            {
                return new AccessPermission(this.RoleId, this.AccessUri);
            }
        }
    }
}