using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.SecurityTokens
{
    public class SecurityTokenAuthenticationFactory : ServiceFactory<WebApplicationBuilder, SecurityTokenAuthenticationFactory.Setting>
    {
        // Constructors
        public SecurityTokenAuthenticationFactory() : base("Authentication", "SecurityTokens", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, SecurityTokenAuthenticationFactory.Setting setting)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentException($"{nameof(applicationBuilder)}=null");
            if (setting == null) throw new ArgumentException($"{nameof(setting)}=null");

            #endregion

            // Require
            if (string.IsNullOrEmpty(setting.Scheme) == true) throw new ArgumentException($"{nameof(setting.Scheme)}=null");
            if (string.IsNullOrEmpty(setting.Header) == true) throw new ArgumentException($"{nameof(setting.Header)}=null");
            //if (string.IsNullOrEmpty(setting.Prefix) == true) throw new ArgumentException($"{nameof(setting.Prefix)}=null");
            if (string.IsNullOrEmpty(setting.Algorithm) == true) throw new ArgumentException($"{nameof(setting.Algorithm)}=null");
            if (string.IsNullOrEmpty(setting.SignKey) == true) throw new ArgumentException($"{nameof(setting.SignKey)}=null");
            //if (string.IsNullOrEmpty(setting.Issuer) == true) throw new ArgumentException($"{nameof(setting.Issuer)}=null");

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // SecurityTokenAuthentication
            authenticationBuilder.AddSecurityTokenAuthentication(setting.Scheme, setting.Header, setting.Prefix, setting.Algorithm, setting.SignKey, setting.Issuer);

            // SecurityTokenAuthenticationSelector
            authenticationBuilder.AddSecurityTokenAuthenticationSelector(setting.Scheme, setting.Header, setting.Prefix);
        }


        // Class
        public class Setting
        {
            // Properties
            public string Scheme { get; set; } = "JwtBearer";

            public string Header { get; set; } = "Authorization";

            public string Prefix { get; set; } = "Bearer";

            public string Algorithm { get; set; } = null;

            public string SignKey { get; set; } = null;

            public string Issuer { get; set; } = null;
        }
    }
}
