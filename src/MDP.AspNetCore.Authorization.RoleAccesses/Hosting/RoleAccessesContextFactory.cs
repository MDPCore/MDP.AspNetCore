using MDP.Registration;
using MDP.RoleAccesses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.AspNetCore.Authorization.RoleAccesses
{
    public class RoleAccessesContextFactory : ServiceFactory<WebApplicationBuilder, RoleAccessesContextFactory.Setting>
    {
        // Constructors
        public RoleAccessesContextFactory() : base("Authorization", "RoleAccesses") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // RoleAccessesContext
            applicationBuilder.Services.TryAddSingleton<RoleAccessesContext>();
        }


        // Class
        public class Setting
        {
            
        }
    }
}
