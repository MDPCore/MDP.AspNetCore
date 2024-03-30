using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Navigation
{
    public class PredicateMenuSelector : MenuSelector
    {
        // Fields
        private readonly string _selectKey = null;

        private readonly Func<Menu, bool> _predicate = null;


        // Constructors
        public PredicateMenuSelector(string selectKey, Func<Menu, bool> predicate)
        {
            #region Contracts

            if (string.IsNullOrEmpty(selectKey) == true) throw new ArgumentException($"{nameof(selectKey)}=null");
            if (predicate == null) throw new ArgumentException($"{nameof(predicate)}=null");

            #endregion

            // Default
            _selectKey = selectKey;
            _predicate= predicate;
        }


        // Properties
        public string SelectKey { get { return _selectKey; } }


        // Methods
        public bool Select(Menu menu)
        {
            #region Contracts

            if (menu == null) throw new ArgumentException($"{nameof(menu)}=null");

            #endregion

            // Return
            return _predicate(menu);
        }
    }
}
