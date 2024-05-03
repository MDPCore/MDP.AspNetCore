using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.OAuthSSO.Server
{
    public class OAuthSSOAuthenticationFactory : ServiceFactory<WebApplicationBuilder, OAuthSSOAuthenticationFactory.Setting>
    {
        // Constructors
        public OAuthSSOAuthenticationFactory() : base("Authentication", "OAuthSSO.Server", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, OAuthSSOAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (setting.ClientCredentials == null) throw new InvalidOperationException($"{nameof(setting.ClientCredentials)}=null");

            // ClientCredential
            foreach(var clientCredential in setting.ClientCredentials)
            {
                // Add
                applicationBuilder.Services.AddSingleton(clientCredential);
            }
        }


        // Class
        public class Setting
        {
            // Properties
            public List<ClientCredential> ClientCredentials { get; set; }
        }
    }
}