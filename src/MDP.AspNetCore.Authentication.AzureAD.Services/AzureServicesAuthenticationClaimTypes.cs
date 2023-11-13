using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public class AzureServicesAuthenticationClaimTypes
    {
        // Constants
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

        public const string ClientId = "appid";
    }
}
