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

            // SecurityToken
            var securityToken = (await _azureCredential.GetTokenAsync(new Azure.Core.TokenRequestContext(scopes), default)).Token;
            if (string.IsNullOrEmpty(securityToken) == true) throw new InvalidOperationException($"{nameof(securityToken)}=null");

            // ViewBag
            this.ViewBag.SecurityToken = securityToken;

            // Return
            return View("Index");
        }
    }

    // GetUser
    public partial class HomeController : Controller
    {
        // Methods
        [Authorize]
        public ActionResult<UserModel> GetUser()
        {
            // ClaimsIdentity
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) throw new InvalidOperationException($"{nameof(claimsIdentity)}=null");
            
            // UserModel
            var userModel = new UserModel();
            userModel.AuthenticationType = claimsIdentity.AuthenticationType;
            userModel.TenantId = claimsIdentity.GetClaimValue(AzureClaimTypes.TenantId);
            userModel.ClientId = claimsIdentity.GetClaimValue(AzureClaimTypes.ClientId);
            userModel.Roles = String.Join(",", claimsIdentity.GetAllClaimValue(System.Security.Claims.ClaimTypes.Role));

            // Return
            return userModel;
        }


        // Class
        public class UserModel
        {
            // Properties
            public string AuthenticationType { get; set; } = string.Empty;

            public string TenantId { get; set; } = string.Empty;

            public string ClientId { get; set; } = string.Empty;

            public string Roles { get; set; } = string.Empty;
        }
    }
}
