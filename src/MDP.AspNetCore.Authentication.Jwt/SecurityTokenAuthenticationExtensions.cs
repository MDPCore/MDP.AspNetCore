using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Net;

namespace MDP.AspNetCore.Authentication.Jwt
{
    public static partial class SecurityTokenAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddSecurityTokenAuthentication(this IServiceCollection services, SecurityTokenAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");

            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new SecurityTokenAuthenticationSetting();
            if (authenticationSetting.Credentials == null) throw new ArgumentException($"{nameof(authenticationSetting.Credentials)}=null");
            foreach (var credential in authenticationSetting.Credentials)
            {
                // Credential
                if (credential == null) throw new ArgumentException($"{nameof(credential)}=null");
                if (string.IsNullOrEmpty(credential.Scheme) == true) throw new ArgumentException($"{nameof(credential.Scheme)}=null");
                if (string.IsNullOrEmpty(credential.Header) == true) throw new ArgumentException($"{nameof(credential.Header)}=null");
                if (string.IsNullOrEmpty(credential.SignKey) == true) throw new ArgumentException($"{nameof(credential.SignKey)}=null");
            }

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // AddSecurityTokenAuthentication
            authenticationBuilder.AddSecurityTokenAuthenticationHandler(authenticationSetting);
            authenticationBuilder.AddSecurityTokenAuthenticationSelector(authenticationSetting);

            // Return
            return authenticationBuilder;
        }
    }

    public static partial class SecurityTokenAuthenticationExtensions
    {
        // Methods
        private static void AddSecurityTokenAuthenticationHandler(this AuthenticationBuilder authenticationBuilder, SecurityTokenAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // CredentialList
            foreach (var credential in authenticationSetting.Credentials)
            {
                // AddSecurityTokenHandler
                authenticationBuilder.AddSecurityTokenAuthenticationHandler(credential);
            }
        }

        private static void AddSecurityTokenAuthenticationHandler(this AuthenticationBuilder authenticationBuilder, SecurityTokenAuthenticationCredential credential)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (credential == null) throw new ArgumentException($"{nameof(credential)}=null");

            #endregion

            // JwtBearer
            authenticationBuilder.AddJwtBearer(credential.Scheme, null, authenticationOptions =>
            {
                // AuthenticationType
                authenticationOptions.TokenValidationParameters.AuthenticationType = credential.Scheme;

                // Issuer
                if (string.IsNullOrEmpty(credential.Issuer) == false)
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuer = true;
                    authenticationOptions.TokenValidationParameters.ValidIssuer = credential.Issuer;
                }
                else
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuer = false;
                    authenticationOptions.TokenValidationParameters.ValidIssuer = null;
                }

                // Audience
                authenticationOptions.TokenValidationParameters.ValidateAudience = false;
                authenticationOptions.TokenValidationParameters.ValidAudience = null;

                // Lifetime
                authenticationOptions.TokenValidationParameters.ValidateLifetime = true;
                authenticationOptions.TokenValidationParameters.ClockSkew = TimeSpan.Zero;

                // SignIng
                if (string.IsNullOrEmpty(credential.SignKey) == false)
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    authenticationOptions.TokenValidationParameters.IssuerSigningKey = CreareSecurityKey(credential.Algorithm, credential.SignKey);
                }
                else
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    authenticationOptions.TokenValidationParameters.IssuerSigningKey = null;
                }

                // SecurityTokenValidators
                {
                    authenticationOptions.SecurityTokenValidators.Clear();
                    authenticationOptions.SecurityTokenValidators.Add(new SecurityTokenHandler());
                }

                // SecurityTokenEvents
                authenticationOptions.Events = new JwtBearerEvents
                {
                    // OnMessageReceived
                    OnMessageReceived = context =>
                    {
                        // Authorization
                        string authorization = context.Request.Headers[credential.Header];
                        if (string.IsNullOrEmpty(authorization) == true)
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        // Token
                        if (string.IsNullOrEmpty(credential.Prefix) == true)
                        {
                            context.Token = authorization;
                        }
                        if (string.IsNullOrEmpty(credential.Prefix) == false && authorization.StartsWith(credential.Prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authorization.Substring(credential.Prefix.Length).Trim();
                        }
                        if (string.IsNullOrEmpty(context.Token) == true)
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        // Return
                        return Task.CompletedTask;
                    },

                    // OnAuthenticationFailed
                    OnAuthenticationFailed = context =>
                    {
                        // Header
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Content
                        var content = JsonSerializer.Serialize(new { message = "Authentication failed" });

                        // Return
                        return context.Response.WriteAsync(content);
                    }
                };
            });
        }

        private static SecurityKey CreareSecurityKey(string algorithm, string signKey)
        {
            #region Contracts

            if (string.IsNullOrEmpty(algorithm) == true) throw new ArgumentException($"{nameof(algorithm)}=null");
            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentException($"{nameof(signKey)}=null");

            #endregion

            // HMAC+SHA
            if (algorithm.StartsWith("HS", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKeyBytes
                var signKeyBytes = Convert.FromBase64String(signKey);
                if (signKeyBytes == null) throw new InvalidOperationException($"{nameof(signKeyBytes)}=null");

                // SecurityKey
                var securityKey = new SymmetricSecurityKey(signKeyBytes);

                // Return
                return securityKey;
            }

            // RSA+SHA
            if (algorithm.StartsWith("RS", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKeyString
                var signKeyString = signKey;
                if (string.IsNullOrEmpty(signKeyString) == true) throw new InvalidOperationException($"{nameof(signKeyString)}=null");

                // RsaKey
                var rsaKey = RSA.Create();
                rsaKey.ImportFromPem(signKeyString);

                // SecurityKey
                var securityKey = new RsaSecurityKey(rsaKey);

                // Return
                return securityKey;
            }

            // ECDSA+SHA
            if (algorithm.StartsWith("ES", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKeyString
                var signKeyString = signKey;
                if (string.IsNullOrEmpty(signKeyString) == true) throw new InvalidOperationException($"{nameof(signKeyString)}=null");

                // EcdsaKey
                var ecdsaKey = ECDsa.Create();
                ecdsaKey.ImportFromPem(signKeyString);

                // SecurityKey
                var securityKey = new ECDsaSecurityKey(ecdsaKey);

                // Return
                return securityKey;
            }

            // Other
            return null;
        }
    }

    public static partial class SecurityTokenAuthenticationExtensions
    {
        // Methods
        private static void AddSecurityTokenAuthenticationSelector(this AuthenticationBuilder authenticationBuilder, SecurityTokenAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // CredentialList
            foreach (var credential in authenticationSetting.Credentials)
            {
                // AddSecurityTokenSelector
                authenticationBuilder.AddSecurityTokenAuthenticationSelector(credential);
            }
        }

        private static void AddSecurityTokenAuthenticationSelector(this AuthenticationBuilder authenticationBuilder, SecurityTokenAuthenticationCredential credential)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (credential == null) throw new ArgumentException($"{nameof(credential)}=null");

            #endregion

            // SecurityTokenPolicyAuthenticationSelector
            var securityTokenAuthenticationSelector = new SecurityTokenAuthenticationSelector(
                scheme: credential.Scheme,
                header: credential.Header,
                prefix: credential.Prefix
            );

            // AddSingleton
            authenticationBuilder.Services.AddSingleton<PolicyAuthenticationSelector>(securityTokenAuthenticationSelector);
        }
    }
}