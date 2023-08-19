using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public class RemoteAuthorizeAttribute : AuthorizeAttribute
    {
        // Constructors
        public RemoteAuthorizeAttribute() : base() 
        {
            // Default
            this.AuthenticationSchemes = RemoteAuthenticationDefaults.AuthenticationScheme;
        }
    }
}
