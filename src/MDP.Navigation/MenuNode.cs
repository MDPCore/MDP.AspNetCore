using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Navigation
{
    public class MenuNode : Menu
    {
        // Constructors
        public MenuNode(Menu menu)
        {
            #region Contracts

            if (menu == null) throw new ArgumentException($"{nameof(menu)}=null");

            #endregion

            // Default
            this.ParentMenuId = menu.ParentMenuId;
            this.MenuId = menu.MenuId;
            this.Name = menu.Name;
            this.Description = menu.Description;
            this.Icon = menu.Icon;
            this.Priority = menu.Priority;
            this.ActionUri = menu.ActionUri;
            this.ResourceUri = menu.ResourceUri;
        }


        // Properties
        public List<MenuNode> Children { get; set; } = new List<MenuNode>();
    }
}
