using MDP.Navigation;
using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Navigation
{
    public class MenuNodeRepositoryFactory : ServiceFactory<WebApplicationBuilder, MenuNodeRepositoryFactory.Setting>
    {
        // Constructors
        public MenuNodeRepositoryFactory() : base("Navigation") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // CacheMenuNodeRepository
            {
                // Register
                applicationBuilder.Services.AddTransient<MenuNodeRepository>((serviceProvider) =>
                {
                    // Create
                    MenuNodeRepository menuNodeRepository = null;
                    menuNodeRepository = new CacheMenuNodeRepository();

                    // Return
                    return menuNodeRepository;
                });
            }
        }


        // Class
        public class Setting
        {

        }
    }
}
