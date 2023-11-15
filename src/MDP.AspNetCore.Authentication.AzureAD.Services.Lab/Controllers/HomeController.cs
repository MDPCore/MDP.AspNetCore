using Azure.Core;
using Azure.Identity;
using MDP.AspNetCore.Authentication.AzureAD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.AzureAD.Service.Lab
{
    // Base
    public partial class HomeController : Controller
    {
        // Fields
        private readonly TokenCredential _azureCredential;

        private readonly string _tenantId = "xxxxx";

        private readonly string _clientId = "xxxxx";

        private readonly string _clientSecret = "xxxxx";

        private readonly string _providerUri = @"api://xxxxx/.default";


        // Constructors
        public HomeController()
        {
            // ClientSecretCredential            
            _azureCredential = new ClientSecretCredential
            (
                tenantId: _tenantId,
                clientId: _clientId,
                clientSecret: _clientSecret
            );

            // DefaultAzureCredential
            //_azureCredential = new DefaultAzureCredential();
        }


        // Methods
        public async Task<ActionResult> Index()
        {
            // Variables
            var scopes = new[]
            {
                _providerUri
            };

            // AccessToken
            var accessToken = (await _azureCredential.GetTokenAsync(new Azure.Core.TokenRequestContext(scopes), default)).Token;
            if (string.IsNullOrEmpty(accessToken) == true) throw new InvalidOperationException($"{nameof(accessToken)}=null");

            // ViewBag
            this.ViewBag.AccessToken = accessToken;

            // Return
            return View("Index");
        }
    }

    // GetClient
    public partial class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult<ClientModel> GetClient()
        {
            // ClaimsIdentity
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");
            
            // ClientModel
            var clientModel = new ClientModel();
            clientModel.AuthenticationType = claimsIdentity.AuthenticationType;
            clientModel.TenantId = claimsIdentity.FindFirst(AzureServicesAuthenticationClaimTypes.TenantId)?.Value;
            clientModel.ClientId = claimsIdentity.FindFirst(AzureServicesAuthenticationClaimTypes.ClientId)?.Value;
            clientModel.Roles = String.Join(",", claimsIdentity.FindAll(System.Security.Claims.ClaimTypes.Role).Select(o => o.Value));

            // Return
            return clientModel;
        }


        // Class
        public class ClientModel
        {
            // Properties
            public string AuthenticationType { get; set; } = string.Empty;

            public string TenantId { get; set; } = string.Empty;

            public string ClientId { get; set; } = string.Empty;

            public string Roles { get; set; } = string.Empty;
        }
    }
}
