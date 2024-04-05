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

namespace MDP.AspNetCore.Authentication.SecurityTokens
{
    public static partial class SecurityTokenAuthenticationExtensions
    {
        // Methods
        public static void AddSecurityTokenAuthentication(this AuthenticationBuilder authenticationBuilder, string scheme, string header, string prefix, string algorithm, string signKey, string issuer)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentException($"{nameof(header)}=null");
            //if (string.IsNullOrEmpty(prefix) == true) throw new ArgumentException($"{nameof(prefix)}=null");
            if (string.IsNullOrEmpty(algorithm) == true) throw new ArgumentException($"{nameof(algorithm)}=null");
            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentException($"{nameof(signKey)}=null");
            //if (string.IsNullOrEmpty(issuer) == true) throw new ArgumentException($"{nameof(issuer)}=null");

            #endregion

            // JwtBearer
            authenticationBuilder.AddJwtBearer(scheme, null, authenticationOptions =>
            {
                // AuthenticationType
                authenticationOptions.TokenValidationParameters.AuthenticationType = scheme;

                // SecurityTokenValidators
                authenticationOptions.AttachSecurityTokenValidators();

                // SecurityTokenEvents
                authenticationOptions.AttachSecurityTokenEvents(header, prefix);

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

        private static JwtBearerOptions AttachSecurityTokenValidators(this JwtBearerOptions securityTokenOptions)
        {
            #region Contracts

            if (securityTokenOptions == null) throw new ArgumentException($"{nameof(securityTokenOptions)}=null");

            #endregion

            // SecurityTokenValidatorArray
            var securityTokenValidatorArray = securityTokenOptions.SecurityTokenValidators.ToArray();
            securityTokenOptions.SecurityTokenValidators.Clear();

            // SecurityTokenValidatorDecoratorList
            foreach (var securityTokenValidator in securityTokenValidatorArray)
            {
                securityTokenOptions.SecurityTokenValidators.Add(new SecurityTokenValidatorDecorator(securityTokenValidator));
            }

            // Return
            return securityTokenOptions;
        }

        private static JwtBearerOptions AttachSecurityTokenEvents(this JwtBearerOptions securityTokenOptions, string authenticationHeader, string authenticationPrefix = null)
        {
            #region Contracts

            if (securityTokenOptions == null) throw new ArgumentException($"{nameof(securityTokenOptions)}=null");
            if (string.IsNullOrEmpty(authenticationHeader) == true) throw new ArgumentException($"{nameof(authenticationHeader)}=null");

            #endregion

            // JwtBearerEvents
            securityTokenOptions.Events ??= new JwtBearerEvents();

            // OnMessageReceived
            var onMessageReceived = securityTokenOptions.Events.OnMessageReceived;
            securityTokenOptions.Events.OnMessageReceived = async context =>
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
            var onAuthenticationFailed = securityTokenOptions.Events.OnAuthenticationFailed;
            securityTokenOptions.Events.OnAuthenticationFailed = async context =>
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
            return securityTokenOptions;
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


        // Class
        private class SecurityTokenValidatorDecorator : ISecurityTokenValidator
        {
            // Fields
            private readonly ISecurityTokenValidator _securityTokenValidator = null;


            // Constructors
            public SecurityTokenValidatorDecorator(ISecurityTokenValidator securityTokenValidator) : base()
            {
                #region Contracts

                if (securityTokenValidator == null) throw new ArgumentException($"{nameof(securityTokenValidator)}=null");

                #endregion

                // Default
                _securityTokenValidator = securityTokenValidator;
            }


            // Properties
            public bool CanValidateToken { get => _securityTokenValidator.CanValidateToken; }

            public int MaximumTokenSizeInBytes { get => _securityTokenValidator.MaximumTokenSizeInBytes; set => _securityTokenValidator.MaximumTokenSizeInBytes = value; }


            // Methods
            public bool CanReadToken(string securityToken) { return _securityTokenValidator.CanReadToken(securityToken); }

            public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validateParameters, out SecurityToken validatedToken)
            {
                #region Contracts

                if (string.IsNullOrEmpty(token) == true) throw new ArgumentException($"{nameof(token)}=null");
                if (validateParameters == null) throw new ArgumentException($"{nameof(validateParameters)}=null");

                #endregion

                // ClaimsPrincipal
                var claimsPrincipal = _securityTokenValidator.ValidateToken(token, validateParameters, out validatedToken);
                if (claimsPrincipal == null) return claimsPrincipal;

                // ClaimsIdentity
                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                if (claimsIdentity == null) return claimsPrincipal;

                // AuthenticationType
                var authenticationType = string.Empty;
                if (string.IsNullOrEmpty(authenticationType) == true) authenticationType = claimsIdentity.FindFirst(AuthenticationClaimTypes.AuthenticationType)?.Value;
                if (string.IsNullOrEmpty(authenticationType) == true) return claimsPrincipal;

                // Create
                claimsIdentity = new ClaimsIdentity(claimsIdentity.Claims, authenticationType);
                claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Return
                return claimsPrincipal;
            }
        }
    }
}