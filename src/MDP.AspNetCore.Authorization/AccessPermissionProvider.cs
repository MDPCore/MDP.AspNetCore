using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authorization
{
    public interface IAccessPermissionProvider
    {
        // Methods
        List<AccessPermission> Create(string roleId, string accessProvider, string accessType);
    }
}