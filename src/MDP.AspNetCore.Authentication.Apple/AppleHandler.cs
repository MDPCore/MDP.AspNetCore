using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace MDP.AspNetCore.Authentication.Apple
{
    public class AppleHandler : OAuthHandler<AppleOptions>
    {
        // Constructors
        public AppleHandler(IOptionsMonitor<AppleOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }


        // Methods
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(nameof(properties));
            ArgumentNullException.ThrowIfNullOrEmpty(nameof(redirectUri));

            #endregion

            // ChallengeUrl
            var challengeUrl = base.BuildChallengeUrl(properties, redirectUri);
            if (string.IsNullOrEmpty(challengeUrl) == true) throw new InvalidOperationException($"{nameof(challengeUrl)}=null");

            // AddQueryString
            challengeUrl = QueryHelpers.AddQueryString(challengeUrl, "response_mode", "form_post");

            // Return
            return challengeUrl;
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            // Require
            if (this.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) == false) throw new InvalidOperationException($"Request.Method={this.Request.Method}");
            if (this.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) == false) throw new InvalidOperationException($"Request.Method={this.Request.ContentType}");

            // Parameters
            var parameters = new Dictionary<string, StringValues>(this.Request.Form, StringComparer.OrdinalIgnoreCase);

            // Code
            var code = string.Empty;
            if (parameters.ContainsKey("code") == true)
            {
                code = parameters["code"];
            }
            if (string.IsNullOrEmpty(code) == true) return HandleRequestResult.Fail("Code was not found.");

            // State
            var state = string.Empty;
            if (parameters.ContainsKey("state") == true)
            {
                state = parameters["state"];
            }
            if (string.IsNullOrEmpty(state) == true) return HandleRequestResult.Fail("State was not found.");

            // Properties
            var properties = this.Options.StateDataFormat.Unprotect(state);
            if (properties == null) return HandleRequestResult.Fail("State was invalid.");
            if (this.ValidateCorrelationId(properties) == false) return HandleRequestResult.Fail("State was invalid.");

            // Tokens
            using (var tokens = await this.ExchangeCodeAsync(new OAuthCodeExchangeContext(properties, code, this.BuildRedirectUri(Options.CallbackPath))))
            {
                // Require
                if (tokens == null) return HandleRequestResult.Fail("Tokens was not found.");
                if (tokens.Error != null) return HandleRequestResult.Fail(tokens.Error);
                if (string.IsNullOrEmpty(tokens.AccessToken) == true) return HandleRequestResult.Fail("AccessToken was not found.");

                // Identity
                var identity = new ClaimsIdentity(this.Options.ClaimsIssuer ?? this.Scheme.Name);
                if (parameters.ContainsKey("user") == true)
                {
                    using (var userDocument = JsonDocument.Parse(parameters["user"]))
                    {
                        // Name
                        var firstName = userDocument.RootElement.GetProperty("name").GetProperty("firstName").GetString();
                        var lastName = userDocument.RootElement.GetProperty("name").GetProperty("lastName").GetString();
                        var name = lastName + firstName;
                        if (string.IsNullOrEmpty(name) == false)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Name, name));
                        }

                        // Mail
                        var mail = userDocument.RootElement.GetProperty("email").GetString();
                        if (string.IsNullOrEmpty(mail) == false)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Email, mail));
                        }
                    }
                }

                // Ticket
                var ticket = await this.CreateTicketAsync(identity, properties, tokens);
                if (ticket != null)
                {
                    return HandleRequestResult.Success(ticket);
                }
                else
                {
                    return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
                }
            }
        }

        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(nameof(context));

            #endregion

            // ClientSecret
            var clientSecret = this.Options.CreateClientSecret();
            if (string.IsNullOrEmpty(clientSecret) == true) throw new InvalidOperationException($"{nameof(clientSecret)}=null");
            this.Options.ClientSecret = clientSecret;

            // Base
            return await base.ExchangeCodeAsync(context);
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            #region Contracts

            ArgumentNullException.ThrowIfNull(nameof(identity));
            ArgumentNullException.ThrowIfNull(nameof(properties));
            ArgumentNullException.ThrowIfNull(nameof(tokens));

            #endregion

            // IdToken
            var idToken = tokens.Response.RootElement.GetProperty("id_token").ToString();
            if (idToken == "null") idToken = string.Empty;
            if (string.IsNullOrEmpty(idToken) == true) throw new InvalidOperationException($"{nameof(idToken)}=null");

            // SecurityToken
            var securityToken = (new JwtSecurityTokenHandler()).ReadJwtToken(idToken);
            if (securityToken == null) throw new InvalidOperationException($"{nameof(securityToken)}=null");

            // Identity
            {
                // NameIdentifier
                if (identity.HasClaim(claim => claim.Type == ClaimTypes.NameIdentifier) == false)
                {
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, securityToken.Subject));
                }

                // Mail
                if (identity.HasClaim(claim => claim.Type == ClaimTypes.Email) == false)
                {
                    var mailClaim = securityToken.Claims.FirstOrDefault(claim => string.Equals(claim.Type, "email", StringComparison.Ordinal));
                    if (mailClaim != null)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Email, mailClaim.Value));
                    }
                }
            }

            // CreatingTicketContext
            var creatingTicketContext = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, this.Context, this.Scheme, this.Options, this.Backchannel, tokens, tokens.Response.RootElement);
            creatingTicketContext.RunClaimActions();
            await Events.CreatingTicket(creatingTicketContext);

            // Return
            return new AuthenticationTicket(creatingTicketContext.Principal, creatingTicketContext.Properties, this.Scheme.Name);
        }
    }
}
