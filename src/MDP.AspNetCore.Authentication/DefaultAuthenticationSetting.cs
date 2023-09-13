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

        public string LoginPath { get; set; } = "/Account/Login";

        public string LogoutPath { get; set; } = "/Account/Logout";

        public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";
    }
}
