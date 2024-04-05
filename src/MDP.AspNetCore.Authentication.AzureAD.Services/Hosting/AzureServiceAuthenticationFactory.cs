using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public class AzureServiceAuthenticationFactory : ServiceFactory<WebApplicationBuilder, AzureServiceAuthenticationFactory.Setting>
    {
        // Constructors
        public AzureServiceAuthenticationFactory() : base("Authentication", "AzureAD.Services", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, AzureServiceAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.Scheme) == true) throw new ArgumentException($"{nameof(setting.Scheme)}=null");
            if (string.IsNullOrEmpty(setting.Header) == true) throw new ArgumentException($"{nameof(setting.Header)}=null");
            //if (string.IsNullOrEmpty(setting.Prefix) == true) throw new ArgumentException($"{nameof(setting.Prefix)}=null");
            if (string.IsNullOrEmpty(setting.TenantId) == true) throw new ArgumentException($"{nameof(setting.TenantId)}=null");
            if (string.IsNullOrEmpty(setting.ClientId) == true) throw new ArgumentException($"{nameof(setting.ClientId)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // AzureServiceAuthentication
            authenticationBuilder.AddAzureServiceAuthentication(setting.Scheme, setting.Header, setting.Prefix, setting.TenantId, setting.ClientId);

            // AzureServiceAuthenticationSelector
            authenticationBuilder.AddAzureServiceAuthenticationSelector(setting.Scheme, setting.Header, setting.Prefix);
        }


        // Class
        public class Setting
        {
            // Properties
            public string Scheme { get; set; } = "AzureAD.Services";

            public string Header { get; set; } = "Authorization";

            public string Prefix { get; set; } = "Bearer";

            public string TenantId { get; set; } = null;

            public string ClientId { get; set; } = null;
        }
    }
}
