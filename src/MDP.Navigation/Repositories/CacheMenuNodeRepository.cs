using CLK.Caching.Memory;
using System;
using System.Collections.Generic;

namespace MDP.Navigation
{
    [MDP.Registration.Service<MenuNodeRepository>()]
    public class CacheMenuNodeRepository : MenuNodeRepository
    {
        // Fields
        private readonly ExpirationMemoryCache _menuNodeCache = new ExpirationMemoryCache();


        // Methods
        public void SetValue(string menuNodeListKey, List<MenuNode> menuNodeList)
        {
            #region Contracts

            if (string.IsNullOrEmpty(menuNodeListKey) == true) throw new ArgumentException($"{nameof(menuNodeListKey)}=null");
            if (menuNodeList == null) throw new ArgumentException($"{nameof(menuNodeList)}=null");

            #endregion

            // SetValue
            _menuNodeCache.SetValue(menuNodeListKey, menuNodeList);
        }

        public bool TryGetValue(string menuNodeListKey, out List<MenuNode> menuNodeList)
        {
            #region Contracts

            if (string.IsNullOrEmpty(menuNodeListKey) == true) throw new ArgumentException($"{nameof(menuNodeListKey)}=null");

            #endregion

            // TryGetValue
            return _menuNodeCache.TryGetValue(menuNodeListKey, out menuNodeList);
        }
    }
}
