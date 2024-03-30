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
        public AuthorizationFactory() : base("Authorization") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AuthorizationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // RoleAuthorizationRequirement
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

            // RoleAuthorizationRequirementHandler
            applicationBuilder.Services.AddSingleton<IAuthorizationHandler,RoleAuthorizationRequirementHandler>();

            // RoleAuthorizationRequirementHandlerSetting
            applicationBuilder.Services.TryAddSingleton(serviceProvider => 
            { 
                // Create
                var roleAuthorizationRequirementHandlerSetting = new RoleAuthorizationRequirementHandlerSetting();
                roleAuthorizationRequirementHandlerSetting.PermissionList = setting.Permissions?.Select(o => o.ToPermission()).ToList();
                
                // Return
                return roleAuthorizationRequirementHandlerSetting;
            });
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
            public Permission ToPermission()
            {
                return new Permission(this.RoleId, this.AccessUri);
            }
        }
    }
}