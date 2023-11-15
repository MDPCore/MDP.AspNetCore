using MDP.AspNetCore.Authentication.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public static partial class AzureServicesAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddAzureServicesAuthentication(this IServiceCollection services, AzureServicesAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");

            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new AzureServicesAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.Scheme) == true) throw new ArgumentException($"{nameof(authenticationSetting.Scheme)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.Header) == true) throw new ArgumentException($"{nameof(authenticationSetting.Header)}=null");
            //if (string.IsNullOrEmpty(authenticationSetting.Prefix) == true) throw new ArgumentException($"{nameof(authenticationSetting.Prefix)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.Instance) == true) throw new ArgumentException($"{nameof(authenticationSetting.Instance)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.TenantId) == true) throw new ArgumentException($"{nameof(authenticationSetting.TenantId)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.ClientId) == true) throw new ArgumentException($"{nameof(authenticationSetting.ClientId)}=null");


            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // AddAzureServicesAuthentication
            authenticationBuilder.AddAzureServicesAuthenticationHandler(authenticationSetting);
            authenticationBuilder.AddAzureServicesAuthenticationSelector(authenticationSetting);

            // Return
            return authenticationBuilder;
        }
    }

    public static partial class AzureServicesAuthenticationExtensions
    {
        // Methods
        private static void AddAzureServicesAuthenticationHandler(this AuthenticationBuilder authenticationBuilder, AzureServicesAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // MicrosoftIdentity
            authenticationBuilder.AddMicrosoftIdentityWebApi
            (
                // SecurityTokenOptions
                securityTokenOptions =>
                {
                    // AuthenticationType
                    securityTokenOptions.TokenValidationParameters.AuthenticationType = authenticationSetting.Scheme;

                    // SecurityTokenValidators
                    securityTokenOptions.AttachSecurityTokenValidators();

                    // SecurityTokenEvents
                    securityTokenOptions.AttachSecurityTokenEvents(authenticationSetting.Header, authenticationSetting.Prefix);
                },

                // MicrosoftIdentityOptions
                microsoftIdentityOptions => 
                {
                    // Credential
                    microsoftIdentityOptions.Instance = authenticationSetting.Instance;
                    microsoftIdentityOptions.TenantId = authenticationSetting.TenantId;
                    microsoftIdentityOptions.ClientId = authenticationSetting.ClientId;
                },

                // AuthenticationScheme
                authenticationSetting.Scheme
            );
        }
    }

    public static partial class AzureServicesAuthenticationExtensions
    {
        // Methods
        private static void AddAzureServicesAuthenticationSelector(this AuthenticationBuilder authenticationBuilder, AzureServicesAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion
                       
            // AzureServicesPolicyAuthenticationSelector
            var securityTokenAuthenticationSelector = new AzureServicesAuthenticationSelector
            (
                scheme: authenticationSetting.Scheme,
                header: authenticationSetting.Header,
                prefix: authenticationSetting.Prefix
            );

            // AddSingleton
            authenticationBuilder.Services.AddSingleton<PolicyAuthenticationSelector>(securityTokenAuthenticationSelector);
        }
    }
}