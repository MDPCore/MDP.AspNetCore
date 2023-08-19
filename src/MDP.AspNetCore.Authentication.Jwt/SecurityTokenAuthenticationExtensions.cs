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

namespace MDP.AspNetCore.Authentication.Jwt
{
    public static class SecurityTokenAuthenticationExtensions
    {
        // Methods
        public static AuthenticationBuilder AddSecurityTokenAuthentication(this IServiceCollection services, SecurityTokenAuthenticationSetting authenticationSetting = null)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");

            #endregion

            // AuthenticationSetting
            if (authenticationSetting == null) authenticationSetting = new SecurityTokenAuthenticationSetting();
            if (string.IsNullOrEmpty(authenticationSetting.Scheme) == true) throw new ArgumentException($"{nameof(authenticationSetting.Scheme)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.Header) == true) throw new ArgumentException($"{nameof(authenticationSetting.Header)}=null");
            if (string.IsNullOrEmpty(authenticationSetting.SignKey) == true) throw new ArgumentException($"{nameof(authenticationSetting.SignKey)}=null");

            // PolicyAuthenticationSelector
            services.AddPolicyAuthenticationSelector(authenticationSetting);

            // JwtBearer
            return services.AddJwtBearer(authenticationSetting);
        }

        private static void AddPolicyAuthenticationSelector(this IServiceCollection services, SecurityTokenAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // SecurityTokenPolicyAuthenticationSelector
            var policyAuthenticationSelector = new SecurityTokenPolicyAuthenticationSelector(
                scheme: authenticationSetting.Scheme,
                header: authenticationSetting.Header,
                prefix: authenticationSetting.Prefix
            );

            // AddSingleton
            services.AddSingleton<PolicyAuthenticationSelector>(policyAuthenticationSelector);
        }

        private static AuthenticationBuilder AddJwtBearer(this IServiceCollection services, SecurityTokenAuthenticationSetting authenticationSetting)
        {
            #region Contracts

            if (services == null) throw new ArgumentException($"{nameof(services)}=null");
            if (authenticationSetting == null) throw new ArgumentException($"{nameof(authenticationSetting)}=null");

            #endregion

            // AuthenticationBuilder   
            var authenticationBuilder = services.AddAuthentication();

            // JwtBearer
            authenticationBuilder.AddJwtBearer(authenticationSetting.Scheme, null, authenticationOptions =>
            {
                // AuthenticationType
                authenticationOptions.TokenValidationParameters.AuthenticationType = authenticationSetting.Scheme;

                // Issuer
                if (string.IsNullOrEmpty(authenticationSetting.Issuer) == false)
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuer = true;
                    authenticationOptions.TokenValidationParameters.ValidIssuer = authenticationSetting.Issuer;
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
                if (string.IsNullOrEmpty(authenticationSetting.SignKey) == false)
                {
                    authenticationOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    authenticationOptions.TokenValidationParameters.IssuerSigningKey = CreateSecurityKey(authenticationSetting.SignKey);
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
                        string authorization = context.Request.Headers[authenticationSetting.Header];
                        if (string.IsNullOrEmpty(authorization) == true)
                        {
                            context.NoResult();
                            return Task.CompletedTask;
                        }

                        // Token
                        if (string.IsNullOrEmpty(authenticationSetting.Prefix) == true)
                        {
                            context.Token = authorization;
                        }
                        if (string.IsNullOrEmpty(authenticationSetting.Prefix) == false && authorization.StartsWith(authenticationSetting.Prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authorization.Substring(authenticationSetting.Prefix.Length).Trim();
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

            // Return
            return authenticationBuilder;
        }

        private static SecurityKey CreateSecurityKey(string signKey)
        {
            #region Contracts

            if (string.IsNullOrEmpty(signKey) == true) throw new ArgumentException($"{nameof(signKey)}=null");

            #endregion

            // RSA
            if (signKey.StartsWith("RSA ", StringComparison.OrdinalIgnoreCase) == true)
            {
                // SignKey
                signKey = signKey.Substring("RSA ".Length).Trim();
                if (string.IsNullOrEmpty(signKey) == false) signKey = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(signKey));
                if (string.IsNullOrEmpty(signKey) == true) throw new InvalidOperationException($"{nameof(signKey)}=null");

                // RsaKey
                var rsa = RSA.Create();
                {
                    // Create
                    rsa.ImportFromPem(signKey);
                    var rsaKey = new RsaSecurityKey(rsa);

                    // Return
                    return rsaKey;
                }
            }

            // Symmetric
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
        }
    }
}
