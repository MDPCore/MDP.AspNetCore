using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;


namespace MDP.AspNetCore.Authentication.AzureAD.Services
{
    public static partial class AzureServiceAuthenticationExtensions
    {
        // Methods
        public static void AddAzureServiceAuthentication(this AuthenticationBuilder authenticationBuilder, string scheme, string header, string prefix, string tenantId, string clientId)
        {
            #region Contracts

            if (authenticationBuilder == null) throw new ArgumentException($"{nameof(authenticationBuilder)}=null");
            if (string.IsNullOrEmpty(scheme) == true) throw new ArgumentException($"{nameof(scheme)}=null");
            if (string.IsNullOrEmpty(header) == true) throw new ArgumentException($"{nameof(header)}=null");
            //if (string.IsNullOrEmpty(prefix) == true) throw new ArgumentException($"{nameof(prefix)}=null");
            if (string.IsNullOrEmpty(tenantId) == true) throw new ArgumentException($"{nameof(tenantId)}=null");
            if (string.IsNullOrEmpty(clientId) == true) throw new ArgumentException($"{nameof(clientId)}=null");

            #endregion

            // MicrosoftIdentity
            authenticationBuilder.AddMicrosoftIdentityWebApi
            (
                // AuthenticationScheme
                jwtBearerScheme: scheme,

                // SecurityTokenOptions
                configureJwtBearerOptions: securityTokenOptions =>
                {
                    // AuthenticationType
                    securityTokenOptions.TokenValidationParameters.AuthenticationType = scheme;

                    // SecurityTokenEvents
                    securityTokenOptions.AttachSecurityTokenEvents(header, prefix);
                },

                // MicrosoftIdentityOptions
                configureMicrosoftIdentityOptions: microsoftIdentityOptions =>
                {
                    // Credential
                    microsoftIdentityOptions.Instance = "https://login.microsoftonline.com/";
                    microsoftIdentityOptions.TenantId = tenantId;
                    microsoftIdentityOptions.ClientId = clientId;
                }
            );
        }

        private static JwtBearerOptions AttachSecurityTokenEvents(this JwtBearerOptions securityTokenOptions, string authenticationHeader, string authenticationPrefix = null)
        {
            #region Contracts

            if (securityTokenOptions == null) throw new ArgumentException($"{nameof(securityTokenOptions)}=null");
            if (string.IsNullOrEmpty(authenticationHeader) == true) throw new ArgumentException($"{nameof(authenticationHeader)}=null");

            #endregion

            // JwtBearerEvents
            securityTokenOptions.Events ??= new JwtBearerEvents();

            // OnTokenValidated 
            var onTokenValidated = securityTokenOptions.Events.OnTokenValidated;
            securityTokenOptions.Events.OnTokenValidated = async context =>
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
    }
}