using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MDP.AspNetCore.Authentication.Jwt
{
    internal static class SecurityTokenOptionsExtensions
    {
        // Methods
        public static JwtBearerOptions AttachSecurityTokenValidators(this JwtBearerOptions securityTokenOptions)
        {
            #region Contracts

            if (securityTokenOptions == null) throw new ArgumentException($"{nameof(securityTokenOptions)}=null");

            #endregion

            // SecurityTokenValidatorArray
            var securityTokenValidatorArray = securityTokenOptions.SecurityTokenValidators.ToArray();
            securityTokenOptions.SecurityTokenValidators.Clear();

            // SecurityTokenHandlerDecoratorList
            foreach (var securityTokenValidator in securityTokenValidatorArray)
            {
                securityTokenOptions.SecurityTokenValidators.Add(new SecurityTokenHandlerDecorator(securityTokenValidator));
            }

            // Return
            return securityTokenOptions;
        }

        public static JwtBearerOptions AttachSecurityTokenEvents(this JwtBearerOptions securityTokenOptions, string authenticationHeader, string authenticationPrefix = null)
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
    }
}
