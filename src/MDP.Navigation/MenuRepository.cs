using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MDP.Navigation
{
    public interface MenuRepository
    {
        // Methods
        List<Menu> FindAll();
    }
}
