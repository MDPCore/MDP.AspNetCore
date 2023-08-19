using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication
{
    public class DefaultAuthenticationSetting
    {
        // Properties
        public string DefaultScheme { get; set; } = LocalAuthenticationDefaults.AuthenticationScheme;

        public string LoginPath { get; set; } = "/Login";

        public string LogoutPath { get; set; } = "/Logout";

        public string AccessDeniedPath { get; set; } = "/AccessDenied";
    }
}
