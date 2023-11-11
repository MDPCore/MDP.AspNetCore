using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MDP.AspNetCore.Authentication.AzureAD.Users
{
    public class AzureUsersHandler : OAuthHandler<AzureUsersOptions>
    {
        // Constructors
        public AzureUsersHandler(IOptionsMonitor<AzureUsersOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }


        // Methods
        protected override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
        {
            // Request
            var request = new HttpRequestMessage(HttpMethod.Post, Options.TokenEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"grant_type", "authorization_code"},
                {"client_id", this.Options.ClientId},
                {"client_secret", this.Options.ClientSecret },
                {"redirect_uri", context.RedirectUri},
                {"code", context.Code}
            });

            // Response
            var response = await this.Backchannel.SendAsync(request, this.Context.RequestAborted);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving AzureUsers user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                // Return
                return OAuthTokenResponse.Success(payload);
            }
        }
        
        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            #region Contracts

            if (identity == null) throw new ArgumentException(nameof(identity));
            if (properties == null) throw new ArgumentException(nameof(properties));
            if (tokens == null) throw new ArgumentException(nameof(tokens));

            #endregion

            // Request
            var request = new HttpRequestMessage(HttpMethod.Get, this.Options.UserInformationEndpoint);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            // Response
            var response = await this.Backchannel.SendAsync(request, this.Context.RequestAborted);
            if (response.IsSuccessStatusCode == false)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(content) == false) throw new HttpRequestException(content);
                if (string.IsNullOrEmpty(content) == true) throw new HttpRequestException($"An error occurred when retrieving AzureUsers user information ({response.StatusCode}). Please check if the authentication information is correct.");
            }

            // Payload
            using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                // CreatingTicketContext
                var creatingTicketContext = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, this.Context, this.Scheme, this.Options, this.Backchannel, tokens, payload.RootElement);
                creatingTicketContext.RunClaimActions();
                await Events.CreatingTicket(creatingTicketContext);

                // AuthenticationTicket
                var authenticationTicket = new AuthenticationTicket(creatingTicketContext.Principal!, creatingTicketContext.Properties, this.Scheme.Name);

                // Return
                return authenticationTicket;
            }
        }
               
        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            #region Contracts

            if (properties == null) throw new ArgumentException(nameof(properties));
            //if (string.IsNullOrEmpty(redirectUri) == true) throw new ArgumentException(nameof(redirectUri));

            #endregion

            // Return
            return QueryHelpers.AddQueryString(this.Options.AuthorizationEndpoint, new Dictionary<string, string>()
            {
                {"response_type", "code"},
                {"scope", this.FormatScope()},
                {"client_id", this.Options.ClientId},
                {"redirect_uri", redirectUri},
                {"state", this.Options.StateDataFormat.Protect(properties)}
            });
        }
    }
}
