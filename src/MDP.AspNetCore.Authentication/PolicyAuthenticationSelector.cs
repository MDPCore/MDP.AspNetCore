using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public interface PolicyAuthenticationSelector
    {
        // Properties
        public string AuthenticationScheme { get; }


        // Methods
        bool Check(HttpContext context);
    }
}
