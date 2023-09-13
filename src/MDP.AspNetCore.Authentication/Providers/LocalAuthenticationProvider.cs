using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public interface LocalAuthenticationProvider
    {
        // Properties
        string RegisterPath { get; set; }


        // Methods
        ClaimsIdentity Login(ClaimsIdentity remoteIdentity);
    }
}
