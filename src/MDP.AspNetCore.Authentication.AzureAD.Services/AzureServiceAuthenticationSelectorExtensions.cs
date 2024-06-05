using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public static partial class AzureServiceAuthenticationSelectorExtensions
    {
        // Methods
        public static void AddAzureServiceAuthenticationSelector(this AuthenticationBuilder authenticationBuilder, string scheme, string header, string prefix)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentNullException($"{nameof(authenticationBuilder)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentNullException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentNullException($"{nameof(header)}=null");
            //if (string.IsNullOrEmpty(prefix) == true) throw new ArgumentNullException($"{nameof(prefix)}=null");

            #endregion

            // AzureServiceAuthenticationSelector
            var azureServiceAuthenticationSelector = new AzureServiceAuthenticationSelector(
                scheme: scheme,
                header: header,
                prefix: prefix
            );

            // AddSingleton
            authenticationBuilder.Services.AddSingleton<PolicyAuthenticationSelector>(azureServiceAuthenticationSelector);
        }
    }
}