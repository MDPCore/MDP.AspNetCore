using MDP.Navigation;
using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Navigation
{
    public class NavigationContextFactory : Factory<WebApplicationBuilder, NavigationContextFactory.Setting>
    {
        // Constructors
        public NavigationContextFactory() : base("Navigation") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // NavigationContext
            {
                // Register
                applicationBuilder.Services.TryAddSingleton<NavigationContext>();
            }
        }


        // Class
        public class Setting
        {

        }
    }
}
