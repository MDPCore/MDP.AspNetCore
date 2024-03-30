using MDP.Navigation;
using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Navigation
{
    public class MenuRepositoryFactory : ServiceFactory<WebApplicationBuilder, MenuRepositoryFactory.Setting>
    {
        // Constructors
        public MenuRepositoryFactory() : base("Navigation") { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // MemoryMenuRepository
            if (setting.Menus != null && setting.Menus.Count > 0)
            {
                // MenuList
                var menuList = setting.Menus;
                menuList.ForEach(o => Validator.ValidateObject(o, new ValidationContext(o)));

                // Register
                applicationBuilder.Services.AddTransient<MenuRepository>((serviceProvider) =>
                {
                    // Create
                    MenuRepository menuRepository = null;
                    menuRepository = new MemoryMenuRepository(menuList);
                    menuRepository = new CacheMenuRepository(menuRepository);

                    // Return
                    return menuRepository;
                });
            }
        }


        // Class
        public class Setting
        {
            // Properties
            public List<Menu> Menus { get; set; } = null;
        }
    }
}
