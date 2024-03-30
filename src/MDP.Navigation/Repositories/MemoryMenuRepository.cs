using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MDP.Navigation
{
    [MDP.Registration.Service<MenuRepository>()]
    public class MemoryMenuRepository : MenuRepository
    {
        // Fields
        private List<Menu> _menuList = null;


        // Constructors
        public MemoryMenuRepository(List<Menu> menuList)
        {
            #region Contracts

            if (menuList == null) throw new ArgumentException($"{nameof(menuList)}=null");

            #endregion

            // Require
            menuList.ForEach(o => Validator.ValidateObject(o, new ValidationContext(o)));

            // Default
            _menuList = menuList;
        }


        // Methods
        public List<Menu> FindAll()
        {
            // FindAll
            return _menuList.ToList();
        }
    }
}
