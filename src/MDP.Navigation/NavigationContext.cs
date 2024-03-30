using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Navigation
{
    [MDP.Registration.Service<NavigationContext>(singleton: true)]
    public class NavigationContext
    {
        // Fields
        private readonly MenuRepository _menuRepository = null;

        private readonly MenuNodeRepository _menuNodeRepository = null;


        // Constructors
        public NavigationContext(MenuRepository menuRepository, MenuNodeRepository menuNodeRepository)
        {
            #region Contracts

            if (menuRepository == null) throw new ArgumentException($"{nameof(menuRepository)}=null");
            if (menuNodeRepository == null) throw new ArgumentException($"{nameof(menuNodeRepository)}=null");

            #endregion

            // Default
            _menuRepository = menuRepository;
            _menuNodeRepository = menuNodeRepository;
        }


        // Methods
        public List<MenuNode> FindAllMenuNode()
        {
            // FindAllMenuNode
            return this.FindAllMenuNode(new PredicateMenuSelector("All", o => true));
        }

        public List<MenuNode> FindAllMenuNode(MenuSelector menuSelector)
        {
            #region Contracts

            if (menuSelector == null) throw new ArgumentException($"{nameof(menuSelector)}=null");

            #endregion

            // Result
            List<MenuNode> menuNodeTree = null;

            // MenuNodeRepository
            if (_menuNodeRepository.TryGetValue(menuSelector.SelectKey, out menuNodeTree) == false)
            {
                // MenuList
                var menuList = _menuRepository.FindAll();
                if (menuList == null) throw new InvalidOperationException($"{nameof(menuList)}=null");

                // Select
                menuList = menuList.Where(o => menuSelector.Select(o)).ToList();
                if (menuList == null) throw new InvalidOperationException($"{nameof(menuList)}=null");

                // MenuNodeTree
                menuNodeTree = this.CreateMenuNodeTree(menuList);
                if (menuNodeTree == null) throw new InvalidOperationException($"{nameof(menuNodeTree)}=null");

                // SetValue
                _menuNodeRepository.SetValue(menuSelector.SelectKey, menuNodeTree);
            }

            // Return
            return menuNodeTree;
        }

        private List<MenuNode> CreateMenuNodeTree(List<Menu> menuList)
        {
            #region Contracts

            if (menuList == null) throw new ArgumentException($"{nameof(menuList)}=null");

            #endregion

            // MenuNodeTree
            var menuNodeTree = new List<MenuNode>();
            {
                // MenuNodeDictionary
                var menuNodeDictionary = new Dictionary<string, MenuNode>();
                foreach (var menu in menuList)
                {
                    menuNodeDictionary.Add(menu.MenuId, new MenuNode(menu));
                }

                // Fill
                foreach (var menuNode in menuNodeDictionary.Values)
                {
                    // Root
                    if (string.IsNullOrEmpty(menuNode.ParentMenuId) == true)
                    {
                        // Add
                        menuNodeTree.Add(menuNode);

                        // Continue
                        continue;
                    }

                    // Child
                    if (menuNodeDictionary.ContainsKey(menuNode.ParentMenuId) == true)
                    {
                        // Add
                        menuNodeDictionary[menuNode.ParentMenuId].Children.Add(menuNode);

                        // Continue
                        continue;
                    }
                }

                // Sort
                foreach (var menuNode in menuNodeDictionary.Values)
                {
                    menuNode.Children = menuNode.Children.OrderBy(o => o.Priority).ToList();
                }
                menuNodeTree = menuNodeTree.OrderBy(o => o.Priority).ToList();
            }

            // Return
            return menuNodeTree;
        }
    }
}
