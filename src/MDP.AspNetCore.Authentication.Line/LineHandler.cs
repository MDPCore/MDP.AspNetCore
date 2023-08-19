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

namespace MDP.AspNetCore.Authentication.Line
{
    public class LineHandler : LineHandler<LineOptions>
    {
        // Constructors
        public LineHandler(IOptionsMonitor<LineOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }
    }

    public class LineHandler<TOptions> : OAuthHandler<TOptions>
        where TOptions : LineOptions, new()
    {
        // Constructors
        public LineHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }


        // Methods
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            #region Contracts

            if (identity == null) throw new ArgumentException(nameof(identity));
            if (properties == null) throw new ArgumentException(nameof(properties));
            if (tokens == null) throw new ArgumentException(nameof(tokens));

            #endregion

            // AccessTokenIdentity
            var accessToken = tokens.AccessToken;
            if (string.IsNullOrEmpty(accessToken) == true) throw new InvalidOperationException($"{nameof(accessToken)} is emptied.");
            if (string.IsNullOrEmpty(accessToken) == false) await this.VerifyAccessToken(accessToken);
            var accessTokenIdentity = await this.CreateAccessTokenIdentity(accessToken);
            if (accessTokenIdentity == null) throw new InvalidOperationException($"{nameof(accessTokenIdentity)}=null");

            // IdTokenIdentity
            var idToken = tokens.Response.RootElement.GetProperty("id_token").ToString();
            if (string.IsNullOrEmpty(idToken) == true) throw new InvalidOperationException($"{nameof(idToken)} is emptied.");
            var idTokenIdentity = await this.CreateIdTokenIdentity(idToken);
            if (idTokenIdentity == null) throw new InvalidOperationException($"{nameof(idTokenIdentity)}=null");

            // NameIdentifier
            var accessTokenNameIdentifier = accessTokenIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accessTokenNameIdentifier) == true) throw new InvalidOperationException($"{nameof(accessTokenNameIdentifier)} is emptied.");
            var idTokenNameIdentifier = idTokenIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idTokenNameIdentifier) == true) throw new InvalidOperationException($"{nameof(idTokenNameIdentifier)} is emptied.");
            if (accessTokenNameIdentifier != idTokenNameIdentifier) throw new InvalidOperationException($"NameIdentifier is failed.");

            // CreatingTicketContext
            OAuthCreatingTicketContext creatingTicketContext = null;
            using (var user = JsonDocument.Parse("{}"))
            {
                creatingTicketContext = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, this.Context, this.Scheme, this.Options, this.Backchannel, tokens, user.RootElement);
                creatingTicketContext.Identity.AddClaims(idTokenIdentity.Claims);
                await Events.CreatingTicket(creatingTicketContext);
            }

            // AuthenticationTicket
            var authenticationTicket = new AuthenticationTicket(creatingTicketContext.Principal!, creatingTicketContext.Properties, this.Scheme.Name);

            // Return
            return authenticationTicket;
        }

        private async Task VerifyAccessToken(string accessToken)
        {
            #region Contracts

            if (string.IsNullOrEmpty(accessToken) == true) throw new ArgumentException($"{nameof(accessToken)}=null");

            #endregion

            // RequestUri
            var requestUriBuilder = new UriBuilder(this.Options.VerifyEndpoint);
            var requestQueryString = HttpUtility.ParseQueryString(requestUriBuilder.Query);
            {
                requestQueryString.Add("access_token", accessToken);
            }
            requestUriBuilder.Query = requestQueryString.ToString();

            var requestUri = requestUriBuilder.ToString();
            if (string.IsNullOrEmpty(requestUri) == true) throw new InvalidOperationException($"{nameof(requestUri)}=null");

            // Request
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            // Response
            var response = await this.Backchannel.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving Line user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                // ClientId
                var clientId = payload.RootElement.GetProperty("client_id").ToString();
                if (string.IsNullOrEmpty(clientId) == true) throw new InvalidOperationException($"{nameof(clientId)} is emptied.");
                if (clientId != this.Options.ClientId) throw new InvalidOperationException($"{nameof(clientId)} is failed.");

                // ExpiresIn
                var expiresIn = payload.RootElement.GetProperty("expires_in").ToString();
                if (string.IsNullOrEmpty(expiresIn) == true) throw new InvalidOperationException($"{nameof(expiresIn)} is emptied.");
                if (int.Parse(expiresIn) <= 0) throw new InvalidOperationException($"{nameof(expiresIn)} is failed.");
            }
        }

        private async Task<ClaimsIdentity> CreateAccessTokenIdentity(string accessToken)
        {
            #region Contracts

            if (string.IsNullOrEmpty(accessToken) == true) throw new ArgumentException($"{nameof(accessToken)}=null");

            #endregion

            // Request
            var request = new HttpRequestMessage(HttpMethod.Get, this.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Response
            var response = await this.Backchannel.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving GitHub user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                // Identity
                var identity = new ClaimsIdentity(this.Options.ClaimsIssuer ?? this.Scheme.Name);
                foreach (var action in this.Options.AccessTokenIdentityClaimActions)
                {
                    action.Run(payload.RootElement, identity, this.Options.ClaimsIssuer ?? this.Scheme.Name);
                }

                // Return
                return identity;
            }
        }

        private async Task<ClaimsIdentity> CreateIdTokenIdentity(string idToken)
        {
            #region Contracts

            if (string.IsNullOrEmpty(idToken) == true) throw new ArgumentException($"{nameof(idToken)}=null");

            #endregion

            // Request
            var request = new HttpRequestMessage(HttpMethod.Post, this.Options.VerifyEndpoint);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "client_id", this.Options.ClientId},
                { "id_token",idToken}
            });

            // Response
            var response = await this.Backchannel.SendAsync(request);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving GitHub user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                // Identity
                var identity = new ClaimsIdentity(this.Options.ClaimsIssuer ?? this.Scheme.Name);
                foreach (var action in this.Options.IdTokenIdentityClaimActions)
                {
                    action.Run(payload.RootElement, identity, this.Options.ClaimsIssuer ?? this.Scheme.Name);
                }

                // Return
                return identity;
            }
        }
    }
}
