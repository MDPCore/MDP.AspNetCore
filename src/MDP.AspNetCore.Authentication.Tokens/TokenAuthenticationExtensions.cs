using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Tokens
{
    public static partial class TokenAuthenticationExtensions
    {
        // Methods
        public static void AddTokenAuthentication(this AuthenticationBuilder authenticationBuilder, string scheme, string header, string prefix, string algorithm, string signKey, string issuer)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentNullException($"{nameof(authenticationBuilder)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentNullException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentNullException($"{nameof(header)}=null");
            //if (string.IsNullOrEmpty(prefix) == true) throw new ArgumentNullException($"{nameof(prefix)}=null");
            if (string.IsNullOrEmpty(algorithm) == true) throw new ArgumentNullException($"{nameof(algorithm)}=null");
            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentNullException($"{nameof(signKey)}=null");
            //if (string.IsNullOrEmpty(issuer) == true) throw new ArgumentNullException($"{nameof(issuer)}=null");

            #endregion

            // JwtBearer
            authenticationBuilder.AddJwtBearer(scheme, null, authenticationOptions =>
            {
                // AuthenticationType
                authenticationOptions.TokenValidationParameters.AuthenticationType = scheme;

                // TokenEvents
                authenticationOptions.AttachTokenEvents(header, prefix);

                // Issuer
                if (string.IsNullOrEmpty(issuer) == false)
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuer = true;
                    authenticationOptions.TokenValidationParameters.ValidIssuer = issuer;
                }
                else
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuer = false;
                    authenticationOptions.TokenValidationParameters.ValidIssuer = null;
                }

                // SigningKey
                authenticationOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
                authenticationOptions.TokenValidationParameters.IssuerSigningKey = CreareSecurityKey(algorithm, signKey);

                // Audience
                authenticationOptions.TokenValidationParameters.ValidateAudience = false;
                authenticationOptions.TokenValidationParameters.ValidAudience = null;

                // Lifetime
                authenticationOptions.TokenValidationParameters.ValidateLifetime = true;
                authenticationOptions.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
            });
        }

        private static JwtBearerOptions AttachTokenEvents(this JwtBearerOptions tokenOptions, string authenticationHeader, string authenticationPrefix = null)
        {
            #region Contracts

            if (tokenOptions == null) throw new ArgumentNullException($"{nameof(tokenOptions)}=null");
            if (string.IsNullOrEmpty(authenticationHeader) == true) throw new ArgumentNullException($"{nameof(authenticationHeader)}=null");

            #endregion

            // JwtBearerEvents
            tokenOptions.Events ??= new JwtBearerEvents();

            // OnTokenValidated 
            var onTokenValidated = tokenOptions.Events.OnTokenValidated;
            tokenOptions.Events.OnTokenValidated = async context =>
            {
                // Base
                if (onTokenValidated != null)
                {
                    await onTokenValidated(context);
                }

                // ClaimsPrincipal
                var claimsPrincipal = context.Principal;
                if (claimsPrincipal == null) return;

                // ClaimsIdentity
                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                if (claimsIdentity == null) return;

                // AuthenticationType
                var authenticationType = string.Empty;
                if (string.IsNullOrEmpty(authenticationType) == true) authenticationType = claimsIdentity.FindFirst(AuthenticationClaimTypes.AuthenticationType)?.Value;
                if (string.IsNullOrEmpty(authenticationType) == true) return;

                // ClaimList
                var claimList = claimsIdentity.Claims.ToList();
                claimList.RemoveAll(o => o.Type == AuthenticationClaimTypes.AuthenticationType);

                // Replace
                claimsIdentity = new ClaimsIdentity(claimList, authenticationType);
                claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.Principal = claimsPrincipal;
            };

            // OnMessageReceived
            var onMessageReceived = tokenOptions.Events.OnMessageReceived;
            tokenOptions.Events.OnMessageReceived = async context =>
            {
                // Base
                if (onMessageReceived != null)
                {
                    await onMessageReceived(context);
                }

                // Authorization
                string authorization = context.Request.Headers[authenticationHeader];
                if (string.IsNullOrEmpty(authorization) == true)
                {
                    context.NoResult();
                    return;
                }

                // Token
                if (string.IsNullOrEmpty(authenticationPrefix) == true)
                {
                    context.Token = authorization;
                }
                if (string.IsNullOrEmpty(authenticationPrefix) == false && authorization.StartsWith(authenticationPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authorization.Substring(authenticationPrefix.Length).Trim();
                }
                if (string.IsNullOrEmpty(context.Token) == true) context.NoResult();
            };

            // OnAuthenticationFailed
            var onAuthenticationFailed = tokenOptions.Events.OnAuthenticationFailed;
            tokenOptions.Events.OnAuthenticationFailed = async context =>
            {
                // Base
                if (onAuthenticationFailed != null)
                {
                    await onAuthenticationFailed(context);
                }

                // Header
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                // Content
                var content = JsonSerializer.Serialize(new { message = "Authentication failed" });

                // Response
                await context.Response.WriteAsync(content);
            };

            // Return
            return tokenOptions;
        }

        private static SecurityKey CreareSecurityKey(string algorithm, string signKey)
        {
            #region Contracts

            if (string.IsNullOrEmpty(algorithm) == true) throw new ArgumentNullException($"{nameof(algorithm)}=null");
            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentNullException($"{nameof(signKey)}=null");

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
}