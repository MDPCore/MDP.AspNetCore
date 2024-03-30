using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Navigation
{
    public interface MenuSelector
    {
        // Properties
        string SelectKey { get; }


        // Methods
        bool Select(Menu menu);
    }
}
