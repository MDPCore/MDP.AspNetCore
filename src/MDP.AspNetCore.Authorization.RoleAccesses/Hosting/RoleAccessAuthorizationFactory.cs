using MDP.Registration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.AspNetCore.Authorization.RoleAccesses
{
    public class RoleAccessAuthorizationFactory : ServiceFactory<WebApplicationBuilder, RoleAccessAuthorizationFactory.Setting>
    {
        // Constructors
        public RoleAccessAuthorizationFactory() : base("Authorization", "RoleAccesses") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // AuthorizationRequirement
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
                policyBuilder.AddRequirements(new RoleAccessRequirement());

                // DefaultPolicy
                options.DefaultPolicy = policyBuilder.Build();
            });

            // AuthorizationRequirementHandler
            {
                // Register
                applicationBuilder.Services.AddSingleton<IAuthorizationHandler, RoleAccessRequirementHandler>();
            }
        }


        // Class
        public class Setting
        {
            
        }
    }
}
