using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public class AzureServicesAuthenticationSetting
    {
        // Properties
        public string Scheme { get; set; } = "AzureAD.Services";

        public string Header { get; set; } = "Authorization";

        public string Prefix { get; set; } = "Bearer";

        public string Instance { get; set; } = "https://login.microsoftonline.com/";

        public string TenantId { get; set; } = null;

        public string ClientId { get; set; } = null;
    }
}
