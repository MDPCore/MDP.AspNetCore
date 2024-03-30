using CLK.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDP.Navigation
{
    [MDP.Registration.Service<MenuRepository>()]
    public class CacheMenuRepository : MenuRepository
    {
        // Fields
        private readonly ExpirationMemoryCache _menuListCache = new ExpirationMemoryCache();

        private readonly MenuRepository _menuRepository = null;


        // Constructors
        public CacheMenuRepository(MenuRepository menuRepository)
        {
            #region Contracts

            if (menuRepository == null) throw new ArgumentException($"{nameof(menuRepository)}=null");

            #endregion

            // Default
            _menuRepository = menuRepository;
        }


        // Methods
        public List<Menu> FindAll()
        {
            // CacheKey
            var cacheKey = $"All";
            if (string.IsNullOrEmpty(cacheKey) == true) throw new InvalidOperationException($"{nameof(cacheKey)}=null");

            // MenuList
            List<Menu> menuList = _menuListCache.GetValue(cacheKey, () =>
            {
                // FindAll
                menuList = _menuRepository.FindAll();
                if (menuList == null) throw new InvalidOperationException($"{nameof(menuList)}=null");

                // Return
                return menuList;
            });
            if (menuList == null) throw new InvalidOperationException($"{nameof(menuList)}=null");

            // FindAll
            return menuList.ToList();
        }
    }
}
