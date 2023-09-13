using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public class LocalAuthorizeAttribute : AuthorizeAttribute
    {
        // Constructors
        public LocalAuthorizeAttribute() : base() 
        {
            // Default

        }
    }
}
