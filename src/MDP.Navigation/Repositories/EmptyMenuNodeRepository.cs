using System;
using System.Collections.Generic;

namespace MDP.Navigation
{
    [MDP.Registration.Service<MenuNodeRepository>()]
    public class EmptyMenuNodeRepository : MenuNodeRepository
    {
        // Methods
        public void SetValue(string menuNodeListKey, List<MenuNode> menuNodeList)
        {
            #region Contracts

            if (string.IsNullOrEmpty(menuNodeListKey) == true) throw new ArgumentException($"{nameof(menuNodeListKey)}=null");
            if (menuNodeList == null) throw new ArgumentException($"{nameof(menuNodeList)}=null");

            #endregion

            // Nothing

        }

        public bool TryGetValue(string menuNodeListKey, out List<MenuNode> menuNodeList)
        {
            #region Contracts

            if (string.IsNullOrEmpty(menuNodeListKey) == true) throw new ArgumentException($"{nameof(menuNodeListKey)}=null");

            #endregion

            // Nothing
            menuNodeList = null;

            // Return
            return false;
        }
    }
}
