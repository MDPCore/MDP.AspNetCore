using MDP.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace MDP.AspNetCore.Authentication.SecurityTokens
{
    public class SecurityTokenAuthenticationFactory : ServiceFactory<WebApplicationBuilder, SecurityTokenAuthenticationFactory.SettingDictionary>
    {
        // Constructors
        public SecurityTokenAuthenticationFactory() : base("Authentication", "SecurityTokens", false) { }


        // Methods
        public override void ConfigureService(WebApplicationBuilder applicationBuilder, SecurityTokenAuthenticationFactory.SettingDictionary settingDictionary)
        {
            #region Contracts

            if (applicationBuilder == null) throw new ArgumentNullException($"{nameof(applicationBuilder)}=null");
            if (settingDictionary == null) throw new ArgumentNullException($"{nameof(settingDictionary)}=null");

            #endregion

            // AuthenticationBuilder   
            var authenticationBuilder = applicationBuilder.Services.AddAuthentication();
            if (authenticationBuilder == null) throw new InvalidOperationException($"{nameof(authenticationBuilder)}=null");

            // SecurityTokenAuthentication
            foreach (var setting in settingDictionary)
            {
                // Require
                if (string.IsNullOrEmpty(setting.Key) == true) throw new InvalidOperationException($"{nameof(setting.Key)}=null");
                if (setting.Value == null) throw new InvalidOperationException($"{nameof(setting.Value)}=null");
                if (string.IsNullOrEmpty(setting.Value.Header) == true) throw new ArgumentNullException($"{nameof(setting.Value.Header)}=null");
                //if (string.IsNullOrEmpty(setting.Value.Prefix) == true) throw new ArgumentNullException($"{nameof(setting.Value.Prefix)}=null");
                if (string.IsNullOrEmpty(setting.Value.Algorithm) == true) throw new ArgumentNullException($"{nameof(setting.Value.Algorithm)}=null");
                if (string.IsNullOrEmpty(setting.Value.SignKey) == true) throw new ArgumentNullException($"{nameof(setting.Value.SignKey)}=null");
                //if (string.IsNullOrEmpty(setting.Issuer) == true) throw new ArgumentNullException($"{nameof(setting.Value.Issuer)}=null");

                // SecurityTokenAuthentication
                authenticationBuilder.AddSecurityTokenAuthentication(setting.Key, setting.Value.Header, setting.Value.Prefix, setting.Value.Algorithm, setting.Value.SignKey, setting.Value.Issuer);

                // SecurityTokenAuthenticationSelector
                authenticationBuilder.AddSecurityTokenAuthenticationSelector(setting.Key, setting.Value.Header, setting.Value.Prefix);
            }
        }


        // Class
        public class SettingDictionary : Dictionary<string, Setting>
        {

        }

        public class Setting
        {
            // Properties
            public string Header { get; set; } = "Authorization";

            public string Prefix { get; set; } = "Bearer";

            public string Algorithm { get; set; } = null;

            public string SignKey { get; set; } = null;

            public string Issuer { get; set; } = null;
        }
    }
}
