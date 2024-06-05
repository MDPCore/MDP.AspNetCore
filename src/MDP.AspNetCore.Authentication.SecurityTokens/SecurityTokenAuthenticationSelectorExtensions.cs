using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace MDP.AspNetCore.Authentication.SecurityTokens
{
    public static partial class SecurityTokenAuthenticationSelectorExtensions
    {
        // Methods
        public static void AddSecurityTokenAuthenticationSelector(this AuthenticationBuilder authenticationBuilder, string scheme, string header, string prefix)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentNullException($"{nameof(authenticationBuilder)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentNullException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentNullException($"{nameof(header)}=null");
            //if (string.IsNullOrEmpty(prefix) == true) throw new ArgumentNullException($"{nameof(prefix)}=null");

            #endregion

            // SecurityTokenPolicyAuthenticationSelector
            var securityTokenAuthenticationSelector = new SecurityTokenAuthenticationSelector(
                scheme: scheme,
                header: header,
                prefix: prefix
            );

            // AddSingleton
            authenticationBuilder.Services.AddSingleton<PolicyAuthenticationSelector>(securityTokenAuthenticationSelector);
        }
    }
}