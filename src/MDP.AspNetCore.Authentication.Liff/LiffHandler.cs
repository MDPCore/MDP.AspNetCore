using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MDP.AspNetCore.Authentication.Line;
using static System.Formats.Asn1.AsnWriter;

namespace MDP.AspNetCore.Authentication.Liff
{
    public class LiffHandler : LineHandler<LiffOptions>
    {
        // Constructors
        public LiffHandler(IOptionsMonitor<LiffOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }


        // Methods
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            #region Contracts

            if (properties == null) throw new ArgumentException(nameof(properties));

            #endregion

            // ReturnUrl
            var returnUrl = string.Empty;
            properties.Items.TryGetValue(".redirect", out returnUrl);
            if (string.IsNullOrEmpty(returnUrl) == true) returnUrl = string.Empty;
                                    
            // ChallengeParameters
            var challengeParameters = new Dictionary<string, string>
            {
                { "authenticationScheme", this.Scheme.Name },
                { "returnUrl", returnUrl }
            };

            // ChallengeUrl
            var challengeUrl = this.Options.ChallengeUrl;
            if (string.IsNullOrEmpty(challengeUrl) == true) throw new InvalidOperationException($"{nameof(challengeUrl)}=null");

            // return
            return QueryHelpers.AddQueryString(challengeUrl, challengeParameters);
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            // AccessToken
            var accessToken = this.Request.Query["access_token"][0];
            if (string.IsNullOrEmpty(accessToken) == true) return HandleRequestResult.Fail("AccessToken was not found.");

            // IdToken
            var idToken = this.Request.Query["id_token"][0];
            if (string.IsNullOrEmpty(idToken) == true) return HandleRequestResult.Fail("IdToken was not found.");

            // ReturnUrl
            var returnUrl = this.Request.Query["returnUrl"][0];
            if (string.IsNullOrEmpty(returnUrl) == true) returnUrl = @"/";

            // Identity
            var identity = new ClaimsIdentity(this.Options.ClaimsIssuer ?? this.Scheme.Name);

            // Properties
            var properties = new AuthenticationProperties() { RedirectUri = returnUrl };

            // Tokens
            var tokens = OAuthTokenResponse.Success(JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                access_token = accessToken,
                id_token = idToken,
            })));

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
}
