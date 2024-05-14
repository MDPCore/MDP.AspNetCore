using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public interface IAccessResourceProvider
    {
        // Methods
        AccessResource Create();
    }
}
